using FioRino_NewProject.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FioRino_NewProject.Responses
{
    public interface IOrderProductsRepository
    {
        Task<DmOrderProduct> GetOrderProductAsync(int OrderProductId);
        Task<List<DmOrderProduct>> GetOrderProductListByOrderProductIdAsync(List<int> OrderproductId);
        Task<List<DmOrderProduct>> GetOrderProductListByGtinAsync(string Gtin);
        Task<List<DmOrderProduct>> GetOrderProductListByOrderIdAsync(int Id);
        void Delete(DmOrderProduct dmOrderProduct);
        Task<DmOrderProduct> InsertProductsToOrderProducts(int OrderId, int ProductId, int SizeId, int SkuId, int CategoryId, int productAmount, string GtinPaging);
    }
}
