using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using static FioRino_NewProject.Controllers.DmOrdersController;

namespace FioRino_NewProject.Services
{
    public interface IOrderProductsService
    {
        Task<Response> PostDmOrderProductsInsertProduct(InsertProductParams parameters);
        Task<Response> DeleteDmOrderProducts(List<int> Ids);
        Task<Response> UpdateProducts(OrderProductDTO dTO);
        Task<Response> PostDmOrdersSendToArchivum(int OrderId);
        

    }
}
