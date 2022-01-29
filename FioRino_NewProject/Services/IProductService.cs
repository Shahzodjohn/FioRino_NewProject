using FioRino_NewProject.Entities;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IProductService
    {
        Task<DmProduct> FindProductAsync(int ProductId);
        Task SettingUpReceiver(DmOrder dmOrder, int CurrentUserId, string SenderName);
        Task AddingProductsToStorage(string GTIN, int SkuCodeId, int Amount, int ProductId, int SizeId, int CategoryId);
        Task<DmProduct> InsertDmProduct(string ProductName, int CategoryId, string Gtin, int UniqueProductId, int sizeId);
    }
}
