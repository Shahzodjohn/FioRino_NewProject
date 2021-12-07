using FioRino_NewProject.Entities;
using FioRino_NewProject.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IOrderService
    {
        Task<Response> DeleteDmOrders(int Id);
        Task<DmOrder> UpdateProductDetails(string OrderExecutor, int SenderId, DateTime CreatedAt, int OrderId);
    }
}
