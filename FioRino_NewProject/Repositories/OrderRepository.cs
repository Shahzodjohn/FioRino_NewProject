using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly FioRinoBaseContext _context;

        public OrderRepository(FioRinoBaseContext context)
        {
            _context = context;
        }

        public async Task DeleteOrder(int OrderId)
        {
            var FindOrder = await _context.DmOrders.FirstOrDefaultAsync(x => x.Id == OrderId);
            _context.DmOrders.Remove(FindOrder);
            await _context.SaveChangesAsync();
        }

        public async Task<DmOrder> FindOrder(int OrderId)
        {
            var FindOrder = await _context.DmOrders.FirstOrDefaultAsync(x => x.Id == OrderId);
            return FindOrder;
        }
    }
}
