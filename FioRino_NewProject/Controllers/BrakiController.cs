using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FioRino_NewProject.Services;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrakiController : ControllerBase
    {
        private readonly IBrakiService _service;

        public BrakiController(IBrakiService service)
        {
            _service = service;
        }

        [HttpGet("BrakiDrukuj")]
        public async Task<IActionResult> BrakiDrukuj(int OrderId)
        {
            await _service.BrakiDrukuj(OrderId);
            return File(await System.IO.File.ReadAllBytesAsync($"wwwroot/ExcelOrderFiles/Order + {OrderId}.xlsx"), "application/octet-stream", $"Order + /{OrderId}.xlsx");
        }
    }
}
