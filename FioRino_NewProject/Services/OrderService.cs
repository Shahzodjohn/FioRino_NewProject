using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Model;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class OrderService : IOrderService
    {
        private readonly FioRinoBaseContext _context;
        private readonly IOrderRepository _oRepository;
        private readonly IOrderProductsRepository _opRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISaveRepository _save;

        public OrderService(FioRinoBaseContext context, IOrderRepository oRepository, IStorageRepository storageRepository, IOrderProductsRepository opRepository, IUserRepository userRepository, ISaveRepository save)
        {
            _context = context;
            _oRepository = oRepository;
            _storageRepository = storageRepository;
            _opRepository = opRepository;
            _userRepository = userRepository;
            _save = save;
        }


        public async Task<Response> DeleteDmOrders(int Id, UserDTO UserInfo)
        {
            var dmOrders = await _oRepository.FindOrder(Id);

            if (UserInfo.RoleName == "Admin")
            {
                var findProduct = await _opRepository.GetOrderProductListByOrderIdAsync(Id);
                if (findProduct.Count == 0)
                {
                    await _oRepository.DeleteOrder(dmOrders.Id);
                    return new Response { Status = "Ok", Message = "Success!" };
                }
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

        public async Task OrdersUpdateIsInMagazynTrue(int OrderId)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var findOrder = await _oRepository.FindOrder(OrderId);
                findOrder.OrderStatusId = 1;
                db.EXPOSE_dm_Orders_UpdateIsInMagazynTrue /**/ (OrderId);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Response> PostDmOrdersCreateOrder(CreateOrderParams parameters)
        {
            int? orderId = 0;
            var findUser = await _userRepository.GetUser(parameters.SenderId);
            parameters.OrderExecutor = "FIORINO Izabela Gądek-Pagacz"; parameters.SourceOfOrder = "Utworzone ręcznie"; parameters.Is_InMagazyn = false;
            parameters.SenderName = findUser.FirstName + " " + findUser.LastName;
            using (SPToCoreContext db = new SPToCoreContext())
            {
                db.EXPOSE_dm_Orders_CreateOrder /**/ (parameters.CreatedAt, parameters.UpdatedAt, parameters.OrderStatusId, parameters.Is_InMagazyn, parameters.SourceOfOrder,
                parameters.OrderExecutor, parameters.SenderName, ref orderId);
            }
            await _context.SaveChangesAsync();
            return new Response { Status = "OK", Message = "OrderId = " + orderId.ToString() };
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
