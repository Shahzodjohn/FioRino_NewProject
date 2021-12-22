using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class OrderProductsService : IOrderProductsService
    {
        private readonly FioRinoBaseContext _context;
        private readonly IOrderProductsRepository _opRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _pRepository;

        public OrderProductsService(FioRinoBaseContext context, IOrderProductsRepository opRepository, IStorageRepository storageRepository, IOrderRepository orderRepository, IProductRepository pRepository)
        {
            _context = context;
            _opRepository = opRepository;
            _storageRepository = storageRepository;
            _orderRepository = orderRepository;
            _pRepository = pRepository;
        }

        public async Task<DmOrderProduct> FindOrderProduct(int OrderProductId)
        {
            var findDmOrderProduct = await _context.DmOrderProducts.FirstOrDefaultAsync(x => x.Id == OrderProductId);
            return findDmOrderProduct;
        }
        public async Task UpdateAmountInStorage(string Gtin,int Amount, int OrderProductId)
        {
            var findOp = await _opRepository.GetOrderProductAsync(OrderProductId);
            var findFromStan = await _context.DmStorages.FirstOrDefaultAsync(x => x.Gtin == Gtin);
            if (findFromStan.AmountLeft >= Amount)
            {
                findOp.ProductStatusesId = 2;
                findFromStan.AmountLeft = findFromStan.AmountLeft - findOp.Amount;
            }
            else
            {
                findOp.ProductStatusesId = 1;
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAmountInStorageAfterDelete(string Gtin)
        {
            var findDmOrderProducts = await _storageRepository.GetOrderProductListAsync(Gtin);
            foreach (var orderProducts in findDmOrderProducts)
            {
                var Order = await _context.DmOrders.FirstOrDefaultAsync(x=>x.Id == orderProducts.OrderId);
                if(Order.IsInArchievum != true)
                {
                    var findStorage = await _context.DmStorages.FirstOrDefaultAsync(x => x.Gtin == Gtin);
                    if (findStorage != null && findStorage.AmountLeft >= orderProducts.Amount)
                    {
                        orderProducts.ProductStatusesId = 2;
                        findStorage.AmountLeft = findStorage.AmountLeft - orderProducts.Amount;
                        await _context.SaveChangesAsync();
                    }
                }
            }

        }
        public async Task<DmStorage> ReturningAmountToStorageBack(int ProductStatusesId, string Gtin, int Amount, int dtoAmount)
        {
            var findOldStorage = await _storageRepository.FindFromStorageByGtinAsync(Gtin);
            if (ProductStatusesId == 2)
            {
                findOldStorage.AmountLeft = findOldStorage.AmountLeft + Amount;

                await _context.SaveChangesAsync();
            }
            return findOldStorage;
        }

        public async Task MinusingBackAfterReturningAmount(List<DmOrderProduct> dmOrderProduct, int OpId)
        {
            foreach (var item in dmOrderProduct)
            {
                var Order = await _context.DmOrders.FirstOrDefaultAsync(x=>x.Id == item.OrderId);
                var findStorage = await _storageRepository.FindFromStorageByGtinAsync(item.Gtin); 
                if (findStorage != null)
                {
                    if (findStorage.AmountLeft >= item.Amount)
                    {
                        //if (item.ProductStatusesId == 2 || item.ProductStatusesId == 1)
                        //{
                            if (item.ProductStatusesId == 1 && Order.IsInArchievum != true|| OpId == item.Id && Order.IsInArchievum != true)
                            {
                                item.ProductStatusesId = 2;
                                findStorage.AmountLeft = findStorage.AmountLeft - item.Amount;
                                await _context.SaveChangesAsync();
                            }
                        //}
                    }
                    else if(OpId == item.Id)
                    {
                        item.ProductStatusesId = 1;
                        await _context.SaveChangesAsync();
                    }
  
                }
            }
        }
        public async Task ManagingStatusesForArchive(int OrderId)
        {
            var findOrderProduct = await _opRepository.GetOrderProductListByOrderIdAsync(OrderId);
            foreach (var item in findOrderProduct)
            {
                var findStorage = await _context.DmStorages.FirstOrDefaultAsync(x => x.Gtin == item.Gtin);
                if(findStorage != null)
                {
                    findStorage.Amount = findStorage.Amount - item.Amount;
                }
                item.ProductStatusesId = 4;
            }
            var findOrder = await _orderRepository.FindOrder(OrderId);
            findOrder.DateOfRelease = DateTime.Now;
        }

        public async Task<Response> ProductValidationForStanController(InsertingProductsParams parameters)
        {
            var SelectCurrentCategory = await _pRepository.FindCategoryAsync(parameters.UniqueProductId, parameters.CategoryId);
            var SelectCurrentSize = await _pRepository.FindSizeAsync(parameters.UniqueProductId, parameters.SizeId);
            if (SelectCurrentCategory == null && SelectCurrentSize == null)
            {
                return new Response { Status = "Error", Message = "Rozmiar i kategoria nie istnieją!" };
            }
            if (SelectCurrentSize == null)
            {
                return new Response { Status = "Error", Message = "Rozmiar nie istnieje!" };
            }
            if (SelectCurrentCategory == null)
            {
                return new Response { Status = "Error", Message = "Kategoria nie istnieje!" };
            }
            return new Response { Status = "Ok", Message = "Success!" };
        }
    }
}
