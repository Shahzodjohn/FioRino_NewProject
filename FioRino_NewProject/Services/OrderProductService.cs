using FioRino_NewProject.Controllers;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Model;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class OrderProductService : IOrderProductsService
    {
        private readonly IOrderProductsRepository _opRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly ISaveRepository _save;
        private readonly IProductRepository _pRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderProductService(IOrderRepository orderRepository, IProductRepository pRepository, ISaveRepository save, IStorageRepository storageRepository, IOrderProductsRepository opRepository)
        {
            _orderRepository = orderRepository;
            _pRepository = pRepository;
            _save = save;
            _storageRepository = storageRepository;
            _opRepository = opRepository;
        }

        public async Task<Response> DeleteDmOrderProducts(List<int> Ids)
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
                    var findDmOrderProducts = await _storageRepository.GetOrderProductListAsync(item.Gtin);
                    foreach (var orderProducts in findDmOrderProducts)
                    {
                        var Order = await _orderRepository.FindOrder(orderProducts.OrderId);
                        if (Order.IsInArchievum != true)
                        {
                            var FindStorage = await _storageRepository.FindFromStorageByGtinAsync(item.Gtin);
                            if (findStorage != null && findStorage.AmountLeft >= orderProducts.Amount)
                            {
                                orderProducts.ProductStatusesId = 2;
                                findStorage.AmountLeft = findStorage.AmountLeft - orderProducts.Amount;
                                await _save.SaveAsync();
                            }
                        }
                    }
                }
                return new Response { Status = "Ok", Message = "Success!" };
            }
            return new Response { Status = "Ok", Message = "Success!" };
        }

        public async Task<Response> PostDmOrderProductsInsertProduct(InsertProductParams parameters)
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
                    return new Response { Status = "Error", Message = "Wybierz odpowiednią ilość!" };
                }
                using (SPToCoreContext db = new SPToCoreContext())
                {
                    db.EXPOSE_dm_OrderProducts_InsertProduct /**/ (parameters.ProductId, parameters.UniqueProductId, parameters.OrderId, parameters.SizeId, parameters.SKUcodeId, parameters.CategoryId, parameters.Amount, ref OrderProductId);
                }
                var findDmOrderProduct = await _opRepository.GetOrderProductAsync(OrderProductId ?? 0);
                findDmOrderProduct.Gtin = SelectingcurrentProduct.Gtin;
                //var findOrder = await _orderService.FindOrder(parameters.OrderId);
                var findOrder = await _orderRepository.FindOrder(parameters.OrderProductId);
                findOrder.Amount = parameters.Amount;
                //var findProduct = await _productService.FindProductAsync(parameters.ProductId);
                var findProduct = await _pRepository.FindProductByIdAsync(parameters.ProductId);
                var findFromStan = await _storageRepository.FindFromStorageByGtinAsync(findProduct.Gtin);
                if (findFromStan != null)
                {
                    var findOp = await _opRepository.GetOrderProductAsync(findDmOrderProduct.Id);
                    if (findFromStan.AmountLeft >= parameters.Amount)
                    {
                        findOp.ProductStatusesId = 2;
                        findFromStan.AmountLeft = findFromStan.AmountLeft - findOp.Amount;
                    }
                    else
                    {
                        findOp.ProductStatusesId = 1;
                    }
                    await _save.SaveAsync();
                }
                else
                    findDmOrderProduct.ProductStatusesId = 1;
                await _save.SaveAsync();
            }
            else
            {
                if (CategoryId == null && SizeId == null)
                {
                    return new Response { Status = "Error", Message = "Rozmiar i kategoria nie istnieją!" };
                }
                if (CategoryId == null)
                {
                    return new Response { Status = "Error", Message = "Kategoria nie istnieje!" };
                }
                if (SizeId == null)
                {
                    return new Response { Status = "Error", Message = "Rozmiar nie istnieje!" };
                }
            }
            return new Response { Status = "Ok", Message = $"{SelectingcurrentProduct.Gtin}" };
        }

        

        public async Task<Response> PostDmOrdersSendToArchivum(int OrderId)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var findOrderProduct = await _opRepository.GetOrderProductListByOrderIdAsync(OrderId);
                foreach (var item in findOrderProduct)
                {
                    var findStorage = await _storageRepository.FindFromStorageByGtinAsync(item.Gtin);
                    if (findStorage != null)
                    {
                        findStorage.Amount = findStorage.Amount - item.Amount;
                    }
                    item.ProductStatusesId = 4;
                }
                var findOrder = await _orderRepository.FindOrder(OrderId);
                findOrder.DateOfRelease = DateTime.Now;
                db.EXPOSE_dm_Orders_SendToArchivum /**/ (OrderId);
                await _save.SaveAsync();
                return new Response { Status = "Ok", Message = "Success!" };
            }
        }

        public async Task<Response> UpdateProducts(OrderProductDTO dTO)
        {
            var SelectingcurrentProduct = await _pRepository.FindProductByParams(dTO.UniqueProductId, dTO.CategoryId ?? 0, dTO.SizeId ?? 0);
            var selectCategory = await _pRepository.FindCategoryAsync(dTO.UniqueProductId, dTO.CategoryId ?? 0);
            var selectSize = await _pRepository.FindSizeAsync(dTO.UniqueProductId, dTO.SizeId ?? 0);
            if (selectCategory == null && selectSize == null)
            {
                return new Response { Status = "Error", Message = "Category and size is incorrect!" };
            }
            if (selectCategory == null)
            {
                return new Response { Status = "Error", Message = "This category does not belong to this product!" };
            }
            if (selectSize == null)
            {
                return new Response { Status = "Error", Message = "This size does not belong to this product!" };
            }
            var findProduct = await _opRepository.GetOrderProductAsync(dTO.Id);
            var findOldStorage = await _storageRepository.FindFromStorageByGtinAsync(findProduct.Gtin);
            if (findProduct.ProductStatusesId == 2)
            {
                findOldStorage.AmountLeft = findOldStorage.AmountLeft + findProduct.Amount;

                await _save.SaveAsync();
            }
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
                    foreach (var item in findOrderProduct)
                    {
                        var Order = await _orderRepository.FindOrder(item.OrderId);
                        var findStorage = await _storageRepository.FindFromStorageByGtinAsync(item.Gtin);
                        if (findStorage != null)
                        {
                            if (findStorage.AmountLeft >= item.Amount)
                            {
                                if (item.ProductStatusesId == 1 && Order.IsInArchievum != true || dTO.Id == item.Id && Order.IsInArchievum != true)
                                {
                                    item.ProductStatusesId = 2;
                                    findStorage.AmountLeft = findStorage.AmountLeft - item.Amount;
                                    await _save.SaveAsync();
                                }   
                            }
                            else if (dTO.Id == item.Id)
                            {
                                item.ProductStatusesId = 1;
                                await _save.SaveAsync();
                            }
                        }
                    }
                }
                else
                {
                    if (findProduct.ProductStatusesId == 1)
                    {
                        findProduct.Amount = dTO.Amount;
                        await _save.SaveAsync();
                        return new Response { Status = "OK", Message = $"Numer zamówienia {findProduct.Id} został pomyślnie zaktualizowany!" };
                    }

                    return new Response { Status = "Error", Message = "Wybierz odpowiednią ilość!" };
                }
            }
            else
            {
                return new Response { Status = "Error", Message = "Wybierz odpowiednią ilość!" };
            }
            return new Response { Status = "OK", Message = $"Numer zamówienia {findProduct.Id} został pomyślnie zaktualizowany!" };
        }
    }
}

