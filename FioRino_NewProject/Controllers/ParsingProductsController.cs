using FioRino_NewProject.Model;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
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
    public class ParsingProductsController : ControllerBase
    {
        private readonly IParsingProductsService _parsingProductsService;

        public ParsingProductsController(IParsingProductsService parsingProductsService)
        {
            _parsingProductsService = parsingProductsService;
        }
        [HttpGet]
        public async Task<IActionResult> ParsingProducts([FromQuery] string cancelToken)
        {
            await _parsingProductsService.ParsingProducts(cancelToken);
            return Ok();
        }
        // EXPOSE_dm_DownloadingStatus_LoadingProcess

        [HttpPost("LoadingProcess")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_DownloadingStatus_LoadingProcessResult>>> PostDmDownloadingStatusLoadingProcess()
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_DownloadingStatus_LoadingProcessAsync /**/ ();
            }
        }
        // EXPOSE_dm_DownloadingStatus_SuccessDate

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
