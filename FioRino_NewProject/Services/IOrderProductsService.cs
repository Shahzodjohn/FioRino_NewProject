using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IOrderProductsService
    {
        Task<DmOrderProduct> FindOrderProduct(int OrderProductId);
        Task UpdateAmountInStorage(string Gtin, int Amount, int OrderProductId);
        Task UpdateAmountInStorageAfterDelete(string Gtin);
        Task<DmStorage> ReturningAmountToStorageBack(int ProductStatusesId, string Gtin, int Amount, int dtoAmount);
        Task MinusingBackAfterReturningAmount(List<DmOrderProduct> dmOrderProduct, int OpId);
        Task ManagingStatusesForArchive(int OrderId);
        Task<Response> ProductValidationForStanController(InsertingProductsParams parameters);
    }
}
