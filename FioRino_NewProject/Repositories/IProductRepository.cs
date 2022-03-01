using FioRino_NewProject.Entities;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public interface IProductRepository
    {
        Task<DmProduct> FindProductByIdAsync(int? Id);
        Task<DmProduct> FindProductByGtinAsync(string Gtin);
        Task<DmProduct> FindProductByParams(int UniqueProductId, int CategoryId, int SizeId);
        Task<DmProduct> FindCategoryAsync(int UniqueProductId, int CategoryId);
        Task<DmProduct> FindSizeAsync(int UniqueProductId, int SizeId);
        Task<DmProduct> FindProductByName(string ProductName);
    }
}
