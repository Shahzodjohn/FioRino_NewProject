using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Responses;
using System;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IOrderService
    {
        Task<Response> DeleteDmOrders(int Id, UserDTO userInfo);
        Task<DmOrder> UpdateProductDetails(string OrderExecutor, int SenderId, DateTime CreatedAt, int OrderId);
        Task OrdersUpdateIsInMagazynTrue(int OrderId);
        Task<Response> PostDmOrdersCreateOrder(CreateOrderParams parameters);
    }
}
