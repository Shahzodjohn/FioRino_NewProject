using FioRino_NewProject.Entities;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IOrderRepository
    {
        Task<DmOrder> FindOrder(int OrderId);
        Task DeleteOrder(int OrderId);
        Task<int> CreateNewOrder(int Id);
        Task UpdateOrder(int amount, int OrderId);
        Task<DmCategory> ClassicCategory();
        Task<DmSize> FindSize(string sizeName);
        Task<int> InsertSize(int sizeNum, string sizeName);
        Task<DmStorage> FindStorage(string GtinPaging);
        Task<DmProduct> FindDmProduct(string GtinPaging);
        Task RemoveOrder(int OrderId);
        Task<int> InsertSKUcodes(string SkuNumber);
        Task<DmProduct> CheckingProductValidation(string productName, int CategoryId, int SizeId);
        Task<int> CreateUniqueProduct(string productNameworksheet);
        Task<int> InsertProductsToDmProducts(string ProductName, int uniqueproductId, int categoryId, int SizeId, string GtinPaging);
        Task<DmSkucode> FindSkuCode(string SkuNumber);
        Task<DmCategory> FasterCategory();
        
    }
}
