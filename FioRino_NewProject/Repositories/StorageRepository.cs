using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public class StorageRepository : IStorageRepository
    {
        private readonly FioRinoBaseContext _context;

        public StorageRepository(FioRinoBaseContext context)
        {
            _context = context;
        }

        public async Task<DmStorage> FindFromStorageByGtinAsync(string Gtin)
        {
            var findFromStan = await _context.DmStorages.FirstOrDefaultAsync(x => x.Gtin == Gtin);
            return findFromStan;
        }
        public async Task<DmStorage> FindFromStorageByIdAsync(int Id)
        {
            var findFromStan = await _context.DmStorages.FirstOrDefaultAsync(x => x.Id == Id);
            return findFromStan;
        }

        public async Task<List<DmOrderProduct>> GetOrderProductListAsync(string Gtin)
        {
            var findFromStan = await _context.DmOrderProducts.Where(x => x.Gtin == Gtin).ToListAsync();
            return findFromStan;
        }
    }
}
