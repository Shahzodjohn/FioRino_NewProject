using FioRino_NewProject.Model;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
using FioRino_NewProject.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        [HttpGet]
        public async Task<IActionResult> ParsingProducts()
        {
            var message = await _parsingProductsService.ParsingProducts();
            if (message.Status == "Ok")
            {
                return Ok(message.Message);
            }
            else
                return BadRequest(message.Message);
            //return Ok();
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
