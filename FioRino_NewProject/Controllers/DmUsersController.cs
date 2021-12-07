using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
using FioRino_web.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DmUsersController : ControllerBase
    {
        private readonly IUserService _uService;

        public DmUsersController(FioRinoBaseContext context, IUserRepository userReposiotory, IUserService uService)
        {
            _uService = uService;
        }
        [HttpPut("UpdateUsersById")]
        public async Task<IActionResult> UpdateUsersById(int id, UpdateUserDTO dmUsers)
        {
            var find = await _uService.CheckValidityEmail(id, dmUsers);
            if(find.Status == "Error")
            {
                return BadRequest(new Response { Status = "Error", Message = $"{find.Message}" });
            }   
            await _uService.UpdateUserById(id, dmUsers);
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
    }
}
