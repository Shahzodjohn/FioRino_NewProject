using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using Microsoft.EntityFrameworkCore;
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
            var findUserAccess = await _context.DmUsersAccesses.FirstOrDefaultAsync(x => x.UserId == id);

            currentUser.FirstName = dmUsers.FirstName;
            currentUser.LastName = dmUsers.LastName;
            currentUser.Email = dmUsers.Email;
            currentUser.PhoneNumber = dmUsers.PhoneNumber;

            currentUser.PositionId = dmUsers.PositionId;
            if (dmUsers.RoleId == 2) { findUserAccess.Hurt = true; findUserAccess.Magazyn = true; findUserAccess.Archive = true; }

            currentUser.RoleId = dmUsers.RoleId;
            await _context.SaveChangesAsync();
            var Valid = dmUsers.Email.Contains("@");
            if (Valid == false)
            {
                return new Response { Status = "Error", Message = "Please insert valid Email Address!" };
            }
            else return new Response { Status = "Ok", Message = "Success!" };
        }

        public async Task<Response> CheckValidityEmail(int id, UpdateUserDTO dmUsers)
        {
            var findUser = await _userRepository.GetByEmail(dmUsers.Email);
            var findbyId = await _userRepository.GetUser(id);
            if (findUser == null || findUser.Email == findbyId.Email)
            {

                var currentUser = await _context.DmUsers.FirstOrDefaultAsync(x => x.Id == id);
                var findUserAccess = await _context.DmUsersAccesses.FirstOrDefaultAsync(x => x.UserId == id);
                currentUser.FirstName = dmUsers.FirstName;
                currentUser.LastName = dmUsers.LastName;
                currentUser.Email = dmUsers.Email;
                currentUser.PhoneNumber = dmUsers.PhoneNumber;
                currentUser.Email = dmUsers.Email;
                currentUser.PositionId = dmUsers.PositionId;
                if (dmUsers.RoleId == 2) { findUserAccess.Hurt = true; findUserAccess.Magazyn = true; findUserAccess.Archive = true; }

                currentUser.RoleId = dmUsers.RoleId;
                await _context.SaveChangesAsync();
                var @Valid = dmUsers.Email.Contains("@");
                if (@Valid == false)
                {
                    return new Response { Status = "Error", Message = "Please insert valid Email Address!" };
                }
                else return new Response { Status = "Ok", Message = "Success!" };
            }
            return new Response { Status = "Error", Message = "Email address is invalid!" };
        }

        public async Task DeleteUser(int Id)
        {
            var User = await _context.DmUsers.FirstOrDefaultAsync(x => x.Id == Id);
            var UserAccess = await _context.DmUsersAccesses.FirstOrDefaultAsync(x => x.UserId == User.Id);
            _context.DmUsersAccesses.Remove(UserAccess);
            _context.DmUsers.Remove(User);
            await _context.SaveChangesAsync();
        }


    }
}
