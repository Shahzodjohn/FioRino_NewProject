using FioRino_NewProject.AccessAttribute;
using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Model;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DmUsersController : ControllerBase
    {
        private readonly IUserService _uService;
        private readonly IUserRepository _userRepository;
        private readonly IUserAccessRepository _accessRepository;

        public DmUsersController(FioRinoBaseContext context, IUserRepository userReposiotory, IUserService uService, IUserRepository userRepository)
        {
            _uService = uService;
            _userRepository = userRepository;
        }

        [HttpPut("UpdateUsersById")]
        public async Task<IActionResult> UpdateUsersById(int id, UpdateUserDTO dmUsers)
        {
            var find = await _uService.CheckValidityEmail(id, dmUsers);
            if(find.Status == "Error")
            {
                return BadRequest(new Response { Status = "Error", Message = $"{find.Message}" });
            }   
            //await _uService.UpdateUserById(id, dmUsers);
            return NoContent();
        }

        [HttpPost("List")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Users_ListResult>>> PostDmUsersList()
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_Users_ListAsync /**/ ();
            }
        }

        [HttpPost("DmUsersAccessByUserId")]
        public async Task<ActionResult> PostDmUsersAccessByUserId([FromBody] ByUserIdParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
               var list = await db.EXPOSE_dm_UsersAccess_ByUserIdAsync /**/ (parameters.UserId);
                return Ok(list);
            }
        }
        
        [HttpPut("UpdateAccesses")]
        public ActionResult PostDmUsersAccessUpdateAccesses([FromBody] UpdateAccessesParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                 db.EXPOSE_dm_UsersAccess_UpdateAccesses /**/ (parameters.UserId ,parameters.Hurt, parameters.Magazyn, parameters.Archive);
                return Ok();
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDmUsers(int id)
        {
            await _uService.DeleteUser(id);
            return NoContent();
        }

        public override bool Equals(object obj)
        {
            return obj is DmUsersController controller &&
                   EqualityComparer<IUserAccessRepository>.Default.Equals(_accessRepository, controller._accessRepository);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_accessRepository);
        }
    }
}
