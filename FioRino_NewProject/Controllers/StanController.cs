using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Model;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StanController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IOrderProductsService _opService;
        private readonly IStorageRepository _storageRepository;
        private readonly IProductRepository _pRepository;
        private readonly IStorageService _storageService;

        public StanController(IWebHostEnvironment environment, IOrderProductsService opService, IStorageRepository storageRepository, IStorageService storageService, IProductRepository pRepository)
        {
            _environment = environment;
            _opService = opService;
            _storageRepository = storageRepository;
            _storageService = storageService;
            _pRepository = pRepository;
        }
        public class listParams
        {
            public string SearchString { get; set; }
        }
        
        [HttpGet("list")]
        public async Task<ActionResult> PostDmStoragelist([FromQuery] listParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var list = await db.EXPOSE_dm_Storage_listAsync /**/ (parameters.SearchString);
                return Ok(list);
            }
        }
        [HttpPost("InsertingProducts")]
        public async Task<ActionResult> PostDmStorageInsertingproducts([FromBody] InsertingProductsParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var SelectingcurrentProduct = await _pRepository.FindProductByParams(parameters.UniqueProductId, parameters.CategoryId, parameters.SizeId);
                var productValidation = await _opService.ProductValidationForStanController(parameters);
                if(productValidation.Status == "Error")
                {
                    return BadRequest(productValidation.Message);
                }
                parameters.ProductId = SelectingcurrentProduct.Id;
                var StorageCheck = await _storageRepository.FindFromStorageByGtinAsync(SelectingcurrentProduct.Gtin);
                if (SelectingcurrentProduct != null && StorageCheck == null)
                {
                    int? stanId = 0;
                    db.EXPOSE_dm_Storage_Insertingproducts /**/ (parameters.UniqueProductId, parameters.SkuCodeId, parameters.ProductId, parameters.CategoryId, parameters.SizeId, parameters.Amount, ref stanId);
                    var findFromStan = await _storageService.UpdatingAmountStorage(stanId ?? 0, parameters, SelectingcurrentProduct.Gtin);
                    if(findFromStan == null)
                    {
                        return BadRequest(new Response { Status = "Error", Message = "Ten produkt jest już w magazynie!" });
                    }
                }
                else if (StorageCheck != null)
                {
                    await _storageService.StorageCheckPlusAmount(StorageCheck, parameters);
                }
                return Ok();
            }
        }
        [HttpPut("MinusingAmount")]
        public async Task<IActionResult> MinusingAmount(StanAmountUpdateDTO dTO)
        {
            var actionStorage = await _storageService.MinusingAmountFromStorage(dTO);
            if(actionStorage.Status == "Error")
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
        public async Task<ActionResult> PostDmStorageBlockList([FromQuery] BlockListParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var list = await db.EXPOSE_dm_Storage_BlockListAsync /**/ (parameters.SearchString);

                return Ok(list);
            }
        }
    }
}
