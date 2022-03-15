using FioRino_NewProject.Entities;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateOrderController : ControllerBase
    {
        
        private readonly UploadingExcelService _uploadingExcelService;

        public CreateOrderController(UploadingExcelService uploadingExcelService)
        {
            _uploadingExcelService = uploadingExcelService;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(IFormFile file, int Id)
        {
            var returnMessage = await _uploadingExcelService.CreateOrder(file, Id);
            if (returnMessage.Status == "Error")
            {
                return BadRequest(new Response { Status = returnMessage.Status, Message = returnMessage.Message });
            }
            else
                return Ok(new Response { Status = returnMessage.Status, Message = returnMessage.Message });
        }
    }
}
