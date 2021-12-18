using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public class StatusRepository : IStatusRepository
    {
        private readonly FioRinoBaseContext _context;

        public StatusRepository(FioRinoBaseContext context)
        {
            _context = context;
        }

        public async Task<DmDownloadingStatus> OriginalValues()
        {
            var originalEntity = await _context.DmDownloadingStatuses.AsNoTracking().FirstOrDefaultAsync();
            return originalEntity;
        }

        public async Task<DmDownloadingStatus> CreateStatusIfNull(int CurrentAmount, int TotalAmount)
        {
            var status = await _context.DmDownloadingStatuses.FirstOrDefaultAsync();
            if(status == null)
            {
                var insert = new DmDownloadingStatus
                {
                     CurrentAmount = CurrentAmount,
                      TotalAmount = TotalAmount
                       //Status = "LOADING"
                };
                await _context.DmDownloadingStatuses.AddAsync(insert);
                await _context.SaveChangesAsync();
                return insert;
            }
            else if (status != null)
            {
                status.CurrentAmount = CurrentAmount;
                status.TotalAmount = TotalAmount;
                //status.Status = "LOADING";
                await _context.SaveChangesAsync();
            }
            return status;
        }

        public async Task<DmDownloadingStatus> GetFirst()
        {
            var first = await _context.DmDownloadingStatuses.FirstOrDefaultAsync();
            return first;
        }
    }
}
