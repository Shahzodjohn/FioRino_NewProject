using FioRino_NewProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoperController : ControllerBase
    {
        private readonly IShoperService _shoperService;
        public ShoperController(IShoperService shoperService)
        {
            _shoperService = shoperService;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task ShoperAction()
        {
            var claim = User.Identity as ClaimsIdentity;
            await _shoperService.ShoperStartParsing(claim);
        }
    }
}


