using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.DataTransferOrigins;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IUserRepository _repository;
        private readonly FioRinoBaseContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public RegisterService(FioRinoBaseContext context, IUserRepository repository, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _context = context;
            _repository = repository;
            _environment = environment;
            _configuration = configuration;
        }

        public async Task<DmUser> CheckingForAuthorization(LoginDTO dTO)
        {
            var user = await _repository.GetByEmail(dTO.Email);
            return user;
        }

        public async Task<DmUser> CheckingUserName(RegisterDTO dto)
        {
            var IsUsedEmail = await _repository.GetByEmail(dto.Email);
            return IsUsedEmail;
        }

        public async Task<DmUser> RegistrationUser(RegisterDTO dto)
        {
            var user = new DmUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                RoleId = dto.RoleId,
                PositionId = dto.PositionId
            };
            _context.DmUsers.Add(user);
            await _context.SaveChangesAsync();
            if (user.RoleId == 1)
            {
                var userAccess = new DmUsersAccess
                {
                    UserId = user.Id,
                    Hurt = user.PositionId == 1 ? true : false,
                    Magazyn = user.PositionId == 2 ? true : false,
                    Archive = false
                };
                _context.DmUsersAccesses.Add(userAccess);
            }
            else if (user.RoleId == 2)
            {
                var userAccess = new DmUsersAccess
                {
                    UserId = user.Id,
                    Hurt = true,
                    Magazyn = true,
                    Archive = true
                };
                _context.DmUsersAccesses.Add(userAccess);
            }

            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<DmUser> GetUserById(int Id)
        {
            var User = await _context.DmUsers.FirstOrDefaultAsync(x => x.Id == Id);
            return User;
        }

        public async Task<DmUser> CreatingDirectory(UploadImageDTO dto)
        {
            var User = await _context.DmUsers.FirstOrDefaultAsync(x => x.Id == dto.Id);
            var rootPath = _environment.WebRootPath;

            var imagesDirectory = rootPath + "/Images/";

            if (!Directory.Exists(imagesDirectory))
                Directory.CreateDirectory(imagesDirectory);

            var filePath = imagesDirectory + dto.File.FileName;

            await using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(fs);
            }

            User.Image = $"/wwwroot/Images/{dto.File.FileName}";
            await _context.SaveChangesAsync();
            return User;
        }

        public async Task<string> JwtSettings(DmUser dto)
        {
            var userExists = await _context.DmUsers.FirstOrDefaultAsync(x => x.Id == dto.Id);
            var userRole = await _context.DmUsers.FirstOrDefaultAsync(x => x.RoleId == userExists.RoleId);
            var userRoles = userRole.RoleId;
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userExists.Email),
                    new Claim(ClaimTypes.NameIdentifier, userExists.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
            authClaims.Add(new Claim(ClaimTypes.Role, userRoles.ToString()));
            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidateAudience"],
                expires: DateTime.Now.AddHours(8),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256));
            var Token = new JwtSecurityTokenHandler().WriteToken(token);
            return Token;
        }

        public async Task<UserDTO> CurrentUser(DmUser currentUser)
        {
            var positionName = (from dmu in _context.DmUsers
                                join dmp in _context.DmPositions on dmu.PositionId equals dmp.Id
                                where dmu.Id == currentUser.Id
                                select dmp.PositionName).ToList().First();
            //дополнить RoleName
            var roleName = (from dmu in _context.DmUsers
                            join dmr in _context.DmRoles on dmu.RoleId equals dmr.Id
                            where dmu.Id == currentUser.Id
                            select dmr.RoleName).ToList().First();
            var UserFind = _context.DmUsersAccesses.FirstOrDefault(x => x.UserId == currentUser.Id);

            var UserInfo = new UserDTO
            {
                Id = currentUser.Id.ToString(),
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                PositionName = positionName,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                RoleName = roleName,
                ImagePath = currentUser.Image,
                HurtAccess = UserFind.Hurt,
                MagazynAccess = UserFind.Magazyn,
                ArchiveAccess = UserFind.Archive,
                Password = "I think you don't have to see the password!"//currentUser.PasswordHash,   
            };
            return UserInfo;
        }

        public async Task<string> VerifyUser(RandomNumberDTO dto)
        {
            var UserEmail = (from c in _context.DmCodesForResetPasswords
                             join u in _context.DmUsers on c.UserId equals u.Id
                             where u.Email.ToLower() == dto.Email.ToLower() && c.RandomNumber == dto.RandomNumber
                             select new { u.Email, c.RandomNumber }).FirstOrDefault();
            return UserEmail.ToString();
        }

        public async Task<DmUser> ResetPassword(NewPasswordDTO dto)
        {
            var currentUser = await _context.DmUsers.FirstOrDefaultAsync(x => x.Email == dto.Email);
            currentUser.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            return currentUser;
        }
    }
}
