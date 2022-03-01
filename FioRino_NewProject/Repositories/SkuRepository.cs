using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public class SkuRepository : ISkuRepository
    {
        private readonly FioRinoBaseContext _context;

        public SkuRepository(FioRinoBaseContext context)
        {
            _context = context;
        }

        public async Task<DmSkucode> FindSkuBySkuCodeName(string value)
        {
            var find = await _context.DmSkucodes.FirstOrDefaultAsync(x => x.SkucodeName == value);
            return find;
        }

        public async Task<DmSkucode> FindSkuBySkuId(int? SkuId)
        {
            var find = await _context.DmSkucodes.FirstOrDefaultAsync(x => x.Id == SkuId);
            return find;
        }

        public async Task<int> InsertingSkuIFNull(string skuCodeName)
        {
            var find = await _context.DmSkucodes.FirstOrDefaultAsync(x=>x.SkucodeName == skuCodeName);
            int skuCodeId = 0;
            if (find == null)
            {
                var addingSkuCode = await _context.DmSkucodes.AddAsync(new DmSkucode
                {
                    SkucodeName = skuCodeName
                });
                await _context.SaveChangesAsync();
                skuCodeId = addingSkuCode.Entity.Id;
            }
            else
            {
                skuCodeId = find.Id;
            }
            return skuCodeId;
        }
    }
}
