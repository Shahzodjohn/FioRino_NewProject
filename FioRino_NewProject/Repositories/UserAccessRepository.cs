using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public class UserAccessRepository : IUserAccessRepository
    {
        private readonly FioRinoBaseContext _context;

        public UserAccessRepository(FioRinoBaseContext context)
        {
            _context = context;
        }

        public async Task<DmUsersAccess> GetById(int Id)
        {
            var user = await _context.DmUsersAccesses.FirstOrDefaultAsync(x=>x.Id == Id);
            return user;
        }
    }
}
