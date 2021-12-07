using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.DataTransferOrigins;
using FioRino_NewProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IRegisterService
    {
        Task<DmUser> RegistrationUser(RegisterDTO dto);
        Task<DmUser> CheckingUserName(RegisterDTO dto);
        Task<DmUser> CheckingForAuthorization(LoginDTO dTO);
        Task<DmUser> GetUserById(int Id);
        Task<DmUser> CreatingDirectory(UploadImageDTO dTO);
        Task<string> JwtSettings(DmUser dto);
        Task<UserDTO> CurrentUser(DmUser dto);
        Task<string> VerifyUser(RandomNumberDTO dto);
        Task<DmUser> ResetPassword(NewPasswordDTO dto);
    }
}
    