using FioRino_NewProject.Data;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class SaveRepository : ISaveRepository
    {
        private readonly FioRinoBaseContext _context;

        public SaveRepository(FioRinoBaseContext context)
        {
            _context = context;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
