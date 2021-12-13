using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
   public class UserRepository : IUserRepository
    {
        public readonly FioRinoBaseContext _context;

        public UserRepository(FioRinoBaseContext context)
        {
            _context = context;
        }

        public async Task<DmUser> Create(DmUser user)
        {
           //var dotValidation = user.Email.Contains("@");
            
             _context.DmUsers.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<DmUser> GetByEmail(string email)
        {
            var find = await _context.DmUsers.FirstOrDefaultAsync(e => e.Email == email);
            return find;
        }

        public async Task<DmUser> GetUser(int Id)
        {
            return await _context.DmUsers.FirstOrDefaultAsync(e => e.Id == Id);
        }

        public async Task<DmUser> Delete(DmUser dmUser)
        {
           var user = _context.DmUsers.FindAsync(dmUser);
           _context.Remove(user);
           await _context.SaveChangesAsync();
           return dmUser;
        }

        public DmUser UserRole(int RoleId)
        {
            var UserRole = _context.DmUsers.FirstOrDefault(x=>x.RoleId == RoleId);
            return UserRole;
        }
   }
}
