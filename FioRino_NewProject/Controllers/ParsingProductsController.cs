using FioRino_NewProject.Model;
using FioRino_NewProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParsingProductsController : ControllerBase
    {
        private readonly ParsingProductsService _parsingProductsService;

        public ParsingProductsController(ParsingProductsService parsingProductsService)
        {
            _parsingProductsService = parsingProductsService;
        }
        [HttpPut("StopParsing")]
        public async Task<IActionResult> StopParsing()
        {
            await _parsingProductsService.Cancel();
            return Ok();
        }
        [HttpPost("LoadingProcess")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_DownloadingStatus_LoadingProcessResult>>> PostDmDownloadingStatusLoadingProcess()
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_DownloadingStatus_LoadingProcessAsync /**/ ();
            }
        }
        [HttpPost("SuccessDate")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_DownloadingStatus_SuccessDateResult>>> PostDmDownloadingStatusSuccessDate()
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_DownloadingStatus_SuccessDateAsync /**/ ();
            }
        }
    }
}
