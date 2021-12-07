using FioRino_NewProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public interface ISkuRepository
    {
        Task<DmSkucode> FindSkuBySkuCodeName(string value);
        Task<int> InsertingSkuIFNull(DmSkucode find,string skuCodeName);
    }
}
