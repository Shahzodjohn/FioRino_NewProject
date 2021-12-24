using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class OrderService : IOrderService
    {
        private readonly FioRinoBaseContext _context;
        private readonly IOrderRepository _oRepository;
        private readonly IOrderProductsRepository _opRepository;
        private readonly IStorageRepository _storageRepository;

        public OrderService(FioRinoBaseContext context, IOrderRepository oRepository, IStorageRepository storageRepository, IOrderProductsRepository opRepository)
        {
            _context = context;
            _oRepository = oRepository;
            _storageRepository = storageRepository;
            _opRepository = opRepository;
        }

        
        public async Task<Response> DeleteDmOrders(int Id, UserDTO UserInfo)
        {
            var dmOrders = await _oRepository.FindOrder(Id);
            
            if(UserInfo.RoleName == "Admin")
            {
                var findProduct = await _opRepository.GetOrderProductListByOrderIdAsync(Id);
                if (findProduct.Count == 0)
                {
                    await _oRepository.DeleteOrder(dmOrders.Id);
                    return new Response();
                }
                foreach (var item in findProduct)
                {
                    foreach (var items in findProduct)
                    {
                        var findStorage = await _storageRepository.FindFromStorageByGtinAsync(items.Gtin);
                        if (findStorage != null && items.ProductStatusesId == 2 || items.ProductStatusesId == 1 || items.ProductStatusesId == 4)
                        {
                            if (items.ProductStatusesId != 1)
                            {
                                findStorage.AmountLeft = findStorage.AmountLeft + items.Amount;
                            }

                            _context.DmOrderProducts.RemoveRange(items);
                            await _context.SaveChangesAsync();
                        }
                    }
                    var findDmOrderProducts = await _opRepository.GetOrderProductListByGtinAsync(item.Gtin);
                    foreach (var orderProducts in findDmOrderProducts)
                    {
                        var Order = await _context.DmOrders.FirstOrDefaultAsync(x => x.Id == orderProducts.OrderId);
                        var findStorage = await _storageRepository.FindFromStorageByGtinAsync(item.Gtin);
                        if (findStorage != null && findStorage.AmountLeft >= orderProducts.Amount && orderProducts.OrderId != dmOrders.Id && orderProducts.ProductStatusesId != 2 && Order.IsInArchievum != true)
                        {
                            orderProducts.ProductStatusesId = 2;
                            findStorage.AmountLeft = findStorage.AmountLeft - orderProducts.Amount;
                            await _context.SaveChangesAsync();
                        }

                    }

                    _context.DmOrders.Remove(dmOrders);
                    await _context.SaveChangesAsync();
                    return new Response { Status = "Ok", Message = "Success!" };
                } 
            }
            else
                return new Response { Status = "Error", Message = "Nie masz prawa do usunięcia pliku WZ!" };

            return new Response { Status = "Ok", Message = "Success!" };
        }

        public async Task<DmOrder> UpdateProductDetails(string OrderExecutor, int SenderId, DateTime CreatedAt, int OrderId)
        {
            var find = await _oRepository.FindOrder(OrderId);
            find.OrderExecutor = OrderExecutor;
            find.SenderId = SenderId;
            find.CreatedAt = CreatedAt;
            find.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return find;
        }
    }
}
