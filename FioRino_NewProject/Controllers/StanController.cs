using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Model;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StanController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public StanController(IStorageService storageService)
        {
            _storageService = storageService;
        }
        [HttpGet("list")]
        public async Task<ActionResult> PostDmStoragelist([FromQuery] string SearchString)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var list = await db.EXPOSE_dm_Storage_listAsync /**/ (SearchString);
                return Ok(list);
            }
        }
        [HttpPost("InsertingProducts")]
        public async Task<ActionResult> PostDmStorageInsertingproducts([FromBody] InsertingProductsParams parameters)
        {
            var actionStorage = await _storageService.StorageInsertingProducts(parameters);
            if (actionStorage.Status == "Error")
            {
                return BadRequest(actionStorage.Message);
            }
            return Ok(new Response { Status = "OK", Message = "Success!" });
            
        }
        [HttpPut("MinusingAmount")]
        public async Task<IActionResult> MinusingAmount(StanAmountUpdateDTO dTO)
        {
            var actionStorage = await _storageService.MinusingAmountFromStorage(dTO);
            if (actionStorage.Status == "Error")
            {
                return BadRequest(actionStorage.Message);
            }
            return Ok(new Response { Status = "OK", Message = "Success!" });
        }

        [HttpPost("SearchingByNameAndSize")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Products_SearchingByNameAndSizeResult>>> PostDmProductsSearchingByCategoryAndSize([FromBody] SearchingByCategoryAndSizeParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_Products_SearchingByNameAndSizeAsync /**/ (parameters.ProductName, parameters.Size);
            }
        }
        [HttpPost("DrukujCody")]
        public async Task<IActionResult> DrukujCody([FromBody] int OrderId)
        {
            await _storageService.DrukujCodes(OrderId);
            return File(await System.IO.File.ReadAllBytesAsync($"PdfCodes/{OrderId}.zip"), "application/octet-stream", OrderId + ".zip");
        }

        public class BlockListParams { public string SearchString { get; set; } }

        [HttpGet("BlockList")]
        public async Task<ActionResult> PostDmStorageBlockList([FromQuery] string SearchString)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var list = await db.EXPOSE_dm_Storage_BlockListAsync /**/ (SearchString);
                return Ok(list);
            }
        }

    }
}
