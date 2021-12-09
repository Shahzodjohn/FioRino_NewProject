using FioRino_NewProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public interface IUserAccessRepository
    {
        Task<DmUsersAccess> GetById(int Id);
    }
}
