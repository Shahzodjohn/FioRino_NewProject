using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public class UniqueProductsRepository : IUniqueProductsRepository
    {
        private readonly FioRinoBaseContext _context;

        public UniqueProductsRepository(FioRinoBaseContext context)
        {
            _context = context;
        }

        public async Task<DmUniqueProduct> FindUniqueProductByName(string PName)
        {
            var containProduct = await _context.DmUniqueProducts.FirstOrDefaultAsync(x => x.ProductName == PName);
            return containProduct;
        }

        public async Task<int> InsertUniqueProductIfNull(string ProductName, int SkuCodeId)
        {
            var dmProduct = await _context.DmUniqueProducts.FirstOrDefaultAsync(x => x.ProductName == ProductName);
            int ProductId = 0;

            if (dmProduct == null && SkuCodeId != 0)
            {
                var newTable = _context.DmUniqueProducts.Add(new DmUniqueProduct
                {
                    ProductName = ProductName,
                    SkuCodeId = SkuCodeId
                });
                await _context.SaveChangesAsync();
                ProductId = newTable.Entity.Id;
            }
            else if(SkuCodeId == 0)
            {
                EntityEntry<DmUniqueProduct> newTable;

                newTable = _context.DmUniqueProducts.Add(new DmUniqueProduct
                {
                    ProductName = ProductName
                });
                await _context.SaveChangesAsync();
                ProductId = newTable.Entity.Id;

                //ProductId = dmProduct.Id;
            }
            else
            {
                ProductId = dmProduct.Id;
            }
            return ProductId;
        }
    }
}
