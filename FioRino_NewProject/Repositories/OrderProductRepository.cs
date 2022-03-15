using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Responses;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public class OrderProductRepository : IOrderProductsRepository
    {
        private readonly FioRinoBaseContext _context;

        public OrderProductRepository(FioRinoBaseContext context)
        {
            _context = context;
        }

        public void Delete(DmOrderProduct dmOrderProduct)
        {
            _context.DmOrderProducts.Remove(dmOrderProduct);
        }

        public async Task<DmOrderProduct> InsertProductsToOrderProducts(int OrderId, int ProductId, int SizeId, int SkuId, int CategoryId, int productAmount, string GtinPaging)
        {
            var insert = new DmOrderProduct
            {
                OrderId = OrderId,
                ProductId = ProductId,
                SizeId = SizeId,
                SkucodeId = SkuId,
                CategoryId = CategoryId,
                Amount = productAmount,
                ProductStatusesId = 1,
                Gtin = GtinPaging
            };
            _context.DmOrderProducts.Add(insert);
            await _context.SaveChangesAsync();

            return insert;
        }
        public async Task<DmOrderProduct> GetOrderProductAsync(int OrderProductId)
        {
            var findDmOrderProduct = await _context.DmOrderProducts.FirstOrDefaultAsync(x => x.Id == OrderProductId);
            return findDmOrderProduct;
        }
        public async Task<List<DmOrderProduct>> GetOrderProductListByGtinAsync(string Gtin)
        {
            var findDmOrderProduct = await _context.DmOrderProducts.Where(x => x.Gtin == Gtin).ToListAsync();
            return findDmOrderProduct;
        }
        public async Task<List<DmOrderProduct>> GetOrderProductListByOrderProductIdAsync(List<int> OrderproductId)
        {
            var findProduct = await _context.DmOrderProducts.Where(x => OrderproductId.Contains(x.Id)).ToListAsync();
            return findProduct;
        }
        public async Task<List<DmOrderProduct>> GetOrderProductListByOrderIdAsync(int Id)
        {
            var findDmOrderProduct = await _context.DmOrderProducts.Where(x => x.OrderId == Id).ToListAsync();
            return findDmOrderProduct;
        }

        public async Task<Response> ProductValidationForStanController(DataTransferObjects.InsertingProductsParams parameters)
        {
           // var SelectCurrentCategory = await _pRepository.FindCategoryAsync(parameters.UniqueProductId, parameters.CategoryId);
            var SelectCurrentCategory = await _context.DmProducts.FirstOrDefaultAsync(x => x.UniqueProductId == parameters.UniqueProductId && x.CategoryId == parameters.CategoryId);
            var SelectCurrentSize = await _context.DmProducts.FirstOrDefaultAsync(x => x.UniqueProductId == parameters.UniqueProductId && x.SizeId == parameters.SizeId);
            if (SelectCurrentCategory == null && SelectCurrentSize == null)
            {
                return new Response { Status = "Error", Message = "Rozmiar i kategoria nie istnieją!" };
            }
            if (SelectCurrentSize == null)
            {
                return new Response { Status = "Error", Message = "Rozmiar nie istnieje!" };
            }
            if (SelectCurrentCategory == null)
            {
                return new Response { Status = "Error", Message = "Kategoria nie istnieje!" };
            }
            return new Response { Status = "Ok", Message = "Success!" };
        }
    }
}
