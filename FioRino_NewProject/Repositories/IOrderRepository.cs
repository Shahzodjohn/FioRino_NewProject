using FioRino_NewProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
   public interface IOrderRepository
   {
        Task<DmOrder> FindOrder(int OrderId);
        Task DeleteOrder(int OrderId);
   }
}
