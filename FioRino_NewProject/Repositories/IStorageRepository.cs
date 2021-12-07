using FioRino_NewProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public interface IStorageRepository
    {
        Task<DmStorage> FindFromStorageByGtinAsync(string Gtin);
        Task<DmStorage> FindFromStorageByIdAsync(int Id);
        Task<List<DmOrderProduct>> GetOrderProductListAsync(string Gtin);
    }
}
