using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Model;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DmOrderProductsController : ControllerBase
    {
        private readonly IOrderProductsService _opService;
        private readonly IOrderRepository _orderService;
        private readonly IProductService _productService;
        private readonly IOrderProductsRepository _opRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly ISaveRepository _save;
        private readonly IProductRepository _pRepository;

        public DmOrderProductsController(IOrderProductsService opService, IOrderRepository orderService, IProductService productService, IOrderProductsRepository opRepository, IStorageRepository storageRepository, ISaveRepository save, IProductRepository pRepository)
        {
            _opService = opService;
            _orderService = orderService;
            _productService = productService;
            _opRepository = opRepository;
            _storageRepository = storageRepository;
            _save = save;
            _pRepository = pRepository;
        }
        
        [HttpPost("InsertProduct")]
        public async Task<ActionResult> PostDmOrderProductsInsertProduct([FromBody] InsertProductParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                int? OrderProductId = 0;
                var SelectingcurrentProduct = await _pRepository.FindProductByParams(parameters.UniqueProductId, parameters.CategoryId, parameters.SizeId);
                var CategoryId = await _pRepository.FindCategoryAsync(parameters.UniqueProductId, parameters.CategoryId);
                var SizeId = await _pRepository.FindCategoryAsync(parameters.UniqueProductId, parameters.SizeId);
                if (SelectingcurrentProduct != null)
                {
                    parameters.ProductId = SelectingcurrentProduct.Id;
                    if (parameters.Amount <= 0)
                    {
                        return BadRequest(new Response { Status = "Error", Message = "Wybierz odpowiednią ilość!" });
                    }
                    db.EXPOSE_dm_OrderProducts_InsertProduct /**/ (parameters.ProductId, parameters.UniqueProductId, parameters.OrderId, parameters.SizeId, parameters.SKUcodeId, parameters.CategoryId, parameters.Amount, ref OrderProductId);
                    var findDmOrderProduct = await _opService.FindOrderProduct(OrderProductId ?? 0);
                    findDmOrderProduct.Gtin = SelectingcurrentProduct.Gtin;
                    var findOrder = await _orderService.FindOrder(parameters.OrderId);
                    findOrder.Amount = parameters.Amount;
                    var findProduct = await _productService.FindProductAsync(parameters.ProductId);
                    var findFromStan = await _storageRepository.FindFromStorageByGtinAsync(findProduct.Gtin);
                    if (findFromStan != null)
                    {
                        await _opService.UpdateAmountInStorage(findFromStan.Gtin, parameters.Amount, findDmOrderProduct.Id);
                    }
                    else
                        findDmOrderProduct.ProductStatusesId = 1;
                    await _save.SaveAsync();
                }
                else
                {
                    if (CategoryId == null && SizeId == null)
                    {
                        return BadRequest(new Response { Status = "Error", Message = "Rozmiar i kategoria nie istnieją!" });
                    }
                    if (CategoryId == null)
                    {
                        return BadRequest(new Response { Status = "Error", Message = "Kategoria nie istnieje!" });
                    }
                    if (SizeId == null)
                    {
                        return BadRequest(new Response { Status = "Error", Message = "Rozmiar nie istnieje!" });
                    }
                }
                return Ok(SelectingcurrentProduct.Gtin);
            }
        }
        [HttpDelete("DeleteRangeById")]
        public async Task<IActionResult> DeleteDmOrderProducts(List<int> Ids)
        {
            var findProduct = await _opRepository.GetOrderProductListByOrderProductIdAsync(Ids);
            foreach (var item in findProduct)
            {
                foreach (var items in findProduct)
                {
                    var findStorage = await _storageRepository.FindFromStorageByGtinAsync(items.Gtin);
                    if (findStorage != null && items.ProductStatusesId == 2 || items.ProductStatusesId == 1)
                    {
                        if (items.ProductStatusesId != 1)
                        {
                            findStorage.AmountLeft = findStorage.AmountLeft + items.Amount;
                        }

                        _opRepository.Delete(items);
                        await _save.SaveAsync();
                    }
                    await _opService.UpdateAmountInStorageAfterDelete(item.Gtin);
                }
                return Ok(new Response { Status = "Ok", Message = "Success!" });
            }
            return NoContent();
        }
        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProducts([FromBody] OrderProductDTO dTO)
        {
            var SelectingcurrentProduct = await _pRepository.FindProductByParams(dTO.UniqueProductId, dTO.CategoryId ?? 0, dTO.SizeId ?? 0);
            var selectCategory = await _pRepository.FindCategoryAsync(dTO.UniqueProductId, dTO.CategoryId ?? 0);
            var selectSize = await _pRepository.FindSizeAsync(dTO.UniqueProductId, dTO.SizeId ?? 0);
            if (selectCategory == null && selectSize == null)
            {
                return BadRequest(new Response { Status = "Error", Message = "Category and size is incorrect!" });
            }
            if (selectCategory == null)
            {
                return BadRequest(new Response { Status = "Error", Message = "This category does not belong to this product!" });
            }
            if (selectSize == null)
            {
                return BadRequest(new Response { Status = "Error", Message = "This size does not belong to this product!" });
            }
            var findProduct = await _opService.FindOrderProduct(dTO.Id);
            await _opService.ReturningAmountToStorageBack(findProduct.ProductStatusesId ?? 0, findProduct.Gtin, findProduct.Amount ?? 0, dTO.Amount ?? 0);

            var findOrderProduct = await _opRepository.GetOrderProductListByGtinAsync(findProduct.Gtin);
            findProduct.ProductId = SelectingcurrentProduct.Id;
            findProduct.Gtin = SelectingcurrentProduct.Gtin;
            findProduct.SizeId = dTO.SizeId;
            findProduct.SkucodeId = dTO.SkUcodeId;
            findProduct.CategoryId = dTO.CategoryId;
            await _save.SaveAsync();
            var findStanProduct = await _storageRepository.FindFromStorageByGtinAsync(findProduct.Gtin);
            if (dTO.Amount > 0)
            {
                if (findStanProduct != null)
                {
                    findProduct.Amount = dTO.Amount;
                    await _opService.MinusingBackAfterReturningAmount(findOrderProduct, dTO.Id);
                }
                else
                {
                    if (findProduct.ProductStatusesId == 1)
                    {
                        findProduct.Amount = dTO.Amount;
                        await _save.SaveAsync();
                        return Ok(new Response { Status = "OK", Message = $"Numer zamówienia {findProduct.Id} został pomyślnie zaktualizowany!" });
                    }

                    return BadRequest(new Response { Status = "Error", Message = "Wybierz odpowiednią ilość!" });
                }
            }
            else
            {
                return BadRequest(new Response { Status = "Error", Message = "Wybierz odpowiednią ilość!" });
            }
            return Ok(new Response { Status = "OK", Message = $"Numer zamówienia {findProduct.Id} został pomyślnie zaktualizowany!" });
        }
        public class GetOrderDetailParams { public int OrderId { get; set; } }
        [HttpPost("GetOrderDetail")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_OrderProducts_GetOrderDetailResult>>> PostDmOrderProductsGetOrderDetail([FromBody] GetOrderDetailParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_OrderProducts_GetOrderDetailAsync /**/ (parameters.OrderId);

            }
        }
    }
}
