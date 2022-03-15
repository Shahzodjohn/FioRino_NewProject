using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.DataTransferOrigins;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IRegisterService _service;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailservice;


        public AuthController(IUserRepository repository, IRegisterService service, IWebHostEnvironment environment, IConfiguration configuration, IMailService mailservice)
        {
            _repository = repository;
            _service = service;
            _configuration = configuration;
            _mailservice = mailservice;
        }

        [HttpPost("RegistrationUser")]
        public async Task<IActionResult> RegistrationUser(RegisterDTO dto)
        {
            var @Valid = dto.Email.Contains("@");
            if (@Valid == false)
            {
                return BadRequest(new Response { Status = "Error", Message = "Please insert valid Email Address!" });
            }

            if ((await _service.CheckingUserName(dto)) != null)
            {
                return BadRequest(new { message = "This email is already used!" });
            }
            await _service.RegistrationUser(dto);
            return Ok();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dTO)
        {
            var userExists = await _repository.GetByEmail(dTO.Email);
            if (userExists == null)
            {
                return BadRequest(new Response { Status = "Error", Message = "This User does not exists!" });
            }
            var userRole = _repository.UserRole(userExists.RoleId ?? 0);
            var user = await _service.CheckingForAuthorization(dTO);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid credentials!" });
            }
            if (!BCrypt.Net.BCrypt.Verify(dTO.Password, user.Password))
            {
                return BadRequest(new { message = "Invalid credentials" });
            }
            #region
            //var key = Encoding.ASCII.GetBytes("my secret key");

            //    var authClaims = new List<Claim>
            //    {
            //        new Claim(ClaimTypes.Name, userExists.Email),
            //        new Claim(ClaimTypes.NameIdentifier, userExists.Id.ToString()),
            //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            //    };
            //authClaims.Add(new Claim(ClaimTypes.Role, userRoles.ToString()));
            //var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            //var token = new JwtSecurityToken(
            //    issuer: _configuration["JWT:ValidIssuer"],
            //    audience: _configuration["JWT:ValidateAudience"],
            //    expires: DateTime.Now.AddHours(8),
            //    claims: authClaims,
            //    signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256));
            //var tokenJWT = new JwtSecurityTokenHandler().WriteToken(token);
            #endregion
            var tokenJWT = await _service.JwtSettings(user);
            return Ok(new
            {
                Status = "Success",
                token = /*"Bearer " +*/tokenJWT,
            });
        }
        [HttpPost("PhotoUpload")]
        public async Task<IActionResult> Upload([FromForm] UploadImageDTO dto)
        {
            try
            {
                var findUser = await _service.GetUserById(dto.Id);
                if (findUser == null)
                {
                    return BadRequest(new Response { Status = "Error", Message = "User is null" });
                }
                var response = await _service.CreatingDirectory(dto);
                return Ok(new Response { Status = "OK", Message = $"The Photo was uploaded successfully! Its address is -> {findUser.Image}" });
            }
            catch (Exception)
            {
                return BadRequest(new Response { Status = "Error", Message = "Error while adding picture" });
            }
        }
        [HttpPost("SendEmailMessage")]
        public async Task<ActionResult> Send([FromBody] MailRequestDTO request)
        {
            //shahzod.akhmedov@bk.ru
            try
            {
                var ValidUser = await _repository.GetByEmail(request.ToEmail);
                if (ValidUser == null)
                {
                    return BadRequest(new Response { Status = "404", Message = "User not found!" });
                }
                await _mailservice.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new Response { Status = "404", Message = "Message was not sent!" });
            }
        }
        [HttpGet("CurrentUser")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CurrentUserAsync()
        {
            var claim = User.Identity as ClaimsIdentity;
            var currentUser = await _repository.GetUser(claim.GetUserId<int>());
            var UserInfo = await _service.CurrentUser(currentUser);
            var userEmail = claim.FindFirst(ClaimTypes.Name)?.Value;
            var user = User.Identity.IsAuthenticated;
            if (UserInfo == null)
                return BadRequest(new Response { Message = "user not found" });
            return Ok(new
            {
                User = UserInfo,
                IsAuthenticated = user
            });
        }
        [HttpPost("VarifyUser")]
        public async Task<ActionResult> VerifyUser(RandomNumberDTO dto)
        {
            var UserEmail = await _service.VerifyUser(dto);
            if (UserEmail == null) { return BadRequest(); };
            return Ok(new Response { Status = "Ok", Message = "Verification success!" });
        }
        [HttpPut("UpdatePassword")]
        public async Task<ActionResult> ResetPassword(NewPasswordDTO dto)
        {
            var reset = await _service.ResetPassword(dto);
            if (reset == null)
            {
                return BadRequest(new Response { Status = "Error", Message = "Password was not updated!" });
            }
            return Ok(new Response { Status = "Success!", Message = "The Password is updated successfully" });
        }

    }
}
