﻿using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Services;
using FioRino_web.Model;
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
    public class DmProductsController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _pService;

        public DmProductsController(IOrderRepository orederRepository, IProductService pService)
        {
            _orderRepository = orederRepository;
            _pService = pService;
        }

        [HttpPost("ByOrderId")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Products_ByOrderIdResult>>> PostDmProductsByOrderId([FromBody] ByOrderIdParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var currentOrder = await _orderRepository.FindOrder(parameters.OrderId);
                var solution = await db.EXPOSE_dm_Products_ByOrderIdAsync /**/ (parameters.OrderId, parameters.CurrentUserId);
                await _pService.SettingUpReceiver(currentOrder, parameters.CurrentUserId, currentOrder.SenderId ?? 0);
                return solution;
            }
        }
        public class SelectingByGtinNumberParams
        {
            public string Gtin { get; set; }
            public int Amount { get; set; }
            public int SkuCodeId { get; set; }
        }
        [HttpPost("SelectingByGtinNumber")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Products_SelectingByGtinNumberResult>>> PostDmProductsSelectingByGtinNumber([FromBody] SelectingByGtinNumberParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var procedure = await db.EXPOSE_dm_Products_SelectingByGtinNumberAsync /**/ (parameters.Gtin, parameters.Amount, parameters.SkuCodeId);
                await _pService.AddingProductsToStorage(procedure[0].GTIN, parameters.SkuCodeId, parameters.Amount, procedure[0].ProductId, procedure[0].SizeId ?? 0, procedure[0].CategoryId ?? 0);
                return Ok(); /*--procedure*/
            }
        }
        [HttpPost("list")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Products_listResult>>> PostDmProductslist()
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_Products_listAsync /**/ ();

            }
        }
    }
}
