using FioRino_NewProject.Entities;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public interface IUniqueProductsRepository
    {
        Task<DmUniqueProduct> FindUniqueProductByName(string PName);
        Task<int> InsertUniqueProductIfNull(string ProductName, int SkuCodeId);
    }
}
