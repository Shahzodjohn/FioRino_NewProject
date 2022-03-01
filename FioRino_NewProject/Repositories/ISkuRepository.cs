using FioRino_NewProject.Entities;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public interface ISkuRepository
    {
        Task<DmSkucode> FindSkuBySkuCodeName(string value);
        Task<DmSkucode> FindSkuBySkuId(int? SkuId);
        Task<int> InsertingSkuIFNull(string skuCodeName);
    }
}
