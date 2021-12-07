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
        public async Task ParsingProducts()
        {
            await _parsingProductsService.ParsingProducts();
        }
    }
}
