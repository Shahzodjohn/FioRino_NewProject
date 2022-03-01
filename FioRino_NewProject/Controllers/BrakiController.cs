using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.AspNetCore.Mvc;
using Spire.Xls;
using System;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;
using System.Linq;
using FioRino_NewProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
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
