using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Model;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

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


