using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly FioRinoBaseContext _context;

        public ProductRepository(FioRinoBaseContext context)
        {
            _context = context;
        }

        public async Task<DmProduct> FindProductByGtinAsync(string Gtin)
        {
            var product = await _context.DmProducts.FirstOrDefaultAsync(x => x.Gtin == Gtin);
            return product;
        }

        public async Task<DmProduct> FindProductByIdAsync(int Id)
        {
            var product = await _context.DmProducts.FirstOrDefaultAsync(x => x.Id == Id);
            return product;
        }
        public async Task<DmProduct> FindProductByParams(int UniqueProductId, int CategoryId, int SizeId)
        {
            var SelectingcurrentProduct = await _context.DmProducts.FirstOrDefaultAsync(x => x.UniqueProductId == UniqueProductId && x.CategoryId == CategoryId && x.SizeId == SizeId);
            return SelectingcurrentProduct;
        }
        public async Task<DmProduct> FindCategoryAsync(int UniqueProductId, int CategoryId)
        {
            var Category = await _context.DmProducts.FirstOrDefaultAsync(x => x.UniqueProductId == UniqueProductId && x.CategoryId == CategoryId);
            return Category;
        }
        public async Task<DmProduct> FindSizeAsync(int UniqueProductId, int SizeId)
        {
            var Size = await _context.DmProducts.FirstOrDefaultAsync(x => x.UniqueProductId == UniqueProductId && x.SizeId == SizeId);
            return Size;
        }

        public async Task<DmProduct> FindProductByName(string ProductName)
        {
            var Product = await _context.DmProducts.FirstOrDefaultAsync(x=>x.ProductName == ProductName);
            return Product;
        }
    }
}
