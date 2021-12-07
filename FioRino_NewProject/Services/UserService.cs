using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class UserService : IUserService
    {
        private readonly FioRinoBaseContext _context;
        private readonly IUserRepository _userRepository;

        public UserService(FioRinoBaseContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task<Response> UpdateUserById(int id, UpdateUserDTO dmUsers)
        {
            var currentUser = await _context.DmUsers.FirstOrDefaultAsync(x => x.Id == id);
            currentUser.FirstName = dmUsers.FirstName;
            currentUser.LastName = dmUsers.LastName;
            currentUser.Email = dmUsers.Email;
            currentUser.PhoneNumber = dmUsers.PhoneNumber;
            currentUser.PositionId = dmUsers.PositionId;
            currentUser.RoleId = dmUsers.RoleId;
            await _context.SaveChangesAsync();
            var @Valid = dmUsers.Email.Contains("@");
            if(@Valid == false)
            {
                return new Response { Status = "Error", Message = "Please insert valid Email Address!" };
            }else return new Response { Status = "Ok", Message = "Success!" };
        }

        public async Task<Response> CheckValidityEmail(int id, UpdateUserDTO dmUsers)
        {
            var findUser = await _userRepository.GetByEmail(dmUsers.Email);
            if (findUser != null)
            {
                var validation = await _context.DmUsers.FirstOrDefaultAsync(x => x.Id != id);
                if (validation != null)
                {
                    return new Response { Status = "Ok", Message = "Success!" };
                }
            }
            return new Response { Status = "Error", Message = "Email address is invalid!" };
        }
    }
}
