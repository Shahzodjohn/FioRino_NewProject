using FioRino_NewProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public interface IStatusRepository
    {
        Task<DmDownloadingStatus> GetFirst();
        Task<DmDownloadingStatus> CreateStatusIfNull(int CurrentAmount, int TotalAmount);
        Task<DmDownloadingStatus> OriginalValues();
    }
}
