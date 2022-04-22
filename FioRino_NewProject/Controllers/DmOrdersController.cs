using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Model;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DmOrdersController : ControllerBase
    {
        private readonly IOrderProductsService _orderProductService;
        private readonly IOrderService _oService;
        private readonly IUserRepository _repository;
        private readonly IRegisterService _service;

        public DmOrdersController(IOrderProductsService orderProductService, IOrderService oService, IUserRepository repository, IRegisterService service)
        {
            _orderProductService = orderProductService;
            _oService = oService;
            _repository = repository;
            _service = service;
        }

        [HttpPost("Amount")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Orders_AmountResult>>> PostDmOrdersAmount()
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_Orders_AmountAsync /**/ ();
            }
        }

        [HttpGet("MagazynList")]
        [Authorize("MagazynAccess")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Orders_MagazynListResult>>> PostDmOrdersMagazynList([FromQuery] int SearchString)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var MagazynListAsync = await db.EXPOSE_dm_Orders_MagazynListAsync /**/ (SearchString);
                return MagazynListAsync;
            }
        }
        
        [HttpGet("List")]
        [Authorize("HurtAccess")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Orders_ListResult>>> PostDmOrdersList([FromQuery] int SearchString)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var Orders_ListAsync = await db.EXPOSE_dm_Orders_ListAsync /**/ (SearchString);
                return Orders_ListAsync;
            }
        }
       
        [HttpGet("ArchiveList")]
        [Authorize("ArchiveAccess")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Orders_ArchiveListResult>>> PostDmOrdersArchiveList([FromQuery] int SearchString)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var Archive = await db.EXPOSE_dm_Orders_ArchiveListAsync /**/ (SearchString);
                return Archive;
            }
        }
        public class OrderParameterDTO { public int OrderId { get; set; } }
        [HttpPut("UpdateIsInMagazynTrue")]
        public async Task<ActionResult> PostDmOrdersUpdateIsInMagazynTrue([FromBody] OrderParameterDTO dto)
        {
            await _oService.OrdersUpdateIsInMagazynTrue(dto.OrderId);
            return Ok();
        }

        [HttpPost("SendToArchivum")]
        public async Task<ActionResult> PostDmOrdersSendToArchivum([FromBody] SendToArchivumDTO dTO)
        {
            var responseMessage = await _orderProductService.PostDmOrdersSendToArchivum(dTO);
            if (responseMessage.Status == "Ok")
            {
                return Ok(new Response { Status = responseMessage.Status, Message = responseMessage.Message });
            }
            else
                return BadRequest(new Response { Status = responseMessage.Status, Message = responseMessage.Message });
        }
        public class SelectFromArchivumParams { public int SearchString { get; set; } }
        [HttpGet("SelectFromArchivum")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Orders_SelectFromArchivumResult>>> PostDmOrdersSelectFromArchivum([FromQuery] int SearchString)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_Orders_SelectFromArchivumAsync /**/ (SearchString);
            }
        }
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteDmOrders(int id)
        {
            var claim = User.Identity as ClaimsIdentity;
            var currentUser = await _repository.GetUser(claim.GetUserId<int>());
            var UserInfo = await _service.CurrentUser(currentUser);

            var message = await _oService.DeleteDmOrders(id, UserInfo);
            if (message.Status == "Ok")
            {
                return Ok(new Response { Status = "Ok", Message = "Success!" });
            }
            else
                return BadRequest(new Response { Status = "Error", Message = "Nie masz prawa do usunięcia pliku WZ!" });

        }
        [HttpPost("CreateOrder")]
        public async Task<ActionResult> PostDmOrdersCreateOrder([FromBody] CreateOrderParams parameters)
        {
            var responseMessage = await _oService.PostDmOrdersCreateOrder(parameters);
            if (responseMessage.Status == "OK")
            {
                return Ok(new Response { Status = responseMessage.Status, Message = responseMessage.Message });
            }
            else
                return BadRequest(new Response { Status = responseMessage.Status, Message = responseMessage.Message });
        }
        [HttpPost("SelectingAllNewOrders")]
        [Authorize("HurtAccess")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Orders_SelectingAllNewOrdersResult>>> PostDmOrdersSelectingAllNewOrders()
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_Orders_SelectingAllNewOrdersAsync /**/ ();
            }
        }
        [HttpPut("UpdateOrderDetails")]
        public async Task<IActionResult> UpdateProductDetails(OrderDetailsUpdate dto)
        {
            var UpdatingEntities = await _oService.UpdateProductDetails(dto.OrderExecutor, dto.SenderId, dto.CreatedAt, dto.OrderId);
            return Ok(new Response { Status = "OK", Message = $"The Order {UpdatingEntities.Id} is updated sucessfully" });
        }

    }
}
