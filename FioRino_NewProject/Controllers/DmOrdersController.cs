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
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISaveRepository _save;
        private readonly IOrderProductsService _orderProductService;
        private readonly IOrderService _oService;
        private readonly IUserRepository _repository;
        private readonly IRegisterService _service;

        public DmOrdersController(IOrderRepository oRepository, ISaveRepository save, IOrderService oService, IOrderProductsService orderProductService, IUserRepository userRepository, IUserRepository repository, IRegisterService service)
        {
            _orderRepository = oRepository;
            _save = save;
            _oService = oService;
            _orderProductService = orderProductService;
            _userRepository = userRepository;
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
        public class MagazynListParams { public int SearchString { get; set; } }

        [HttpGet("MagazynList")]
        [Authorize("MagazynAccess")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Orders_MagazynListResult>>> PostDmOrdersMagazynList([FromQuery] MagazynListParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var MagazynListAsync = await db.EXPOSE_dm_Orders_MagazynListAsync /**/ (parameters.SearchString);
                return MagazynListAsync;
            }
        }
        public class ListParams { public int SearchString { get; set; } }
        //// EXPOSE_dm_Orders_List

        [HttpGet("List")]
        [Authorize("HurtAccess")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Orders_ListResult>>> PostDmOrdersList([FromQuery] ListParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var Orders_ListAsync = await db.EXPOSE_dm_Orders_ListAsync /**/ (parameters.SearchString);
                return Orders_ListAsync;
            }
        }
        public class ArchiveListParams { public int SearchString { get; set; } }
        // EXPOSE_dm_Orders_ArchiveList
        [HttpGet("ArchiveList")]
        [Authorize("ArchiveAccess")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Orders_ArchiveListResult>>> PostDmOrdersArchiveList([FromQuery] ArchiveListParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var Archive = await db.EXPOSE_dm_Orders_ArchiveListAsync /**/ (parameters.SearchString);
                return Archive;
            }
        }
        public class UpdateIsInMagazynTrueParams { public int OrderId { get; set; } }
        // EXPOSE_dm_Orders_UpdateIsInMagazynTrue

        [HttpPut("UpdateIsInMagazynTrue")]
        public async Task<ActionResult> PostDmOrdersUpdateIsInMagazynTrue([FromBody] UpdateIsInMagazynTrueParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                var findOrder = await _orderRepository.FindOrder(parameters.OrderId);
                findOrder.OrderStatusId = 1;
                db.EXPOSE_dm_Orders_UpdateIsInMagazynTrue /**/ (parameters.OrderId);
                await _save.SaveAsync();
                return Ok();
            }
        }
        public class SendToArchivumParams { public int OrderId { get; set; } }
        // EXPOSE_dm_Orders_SendToArchivum

        [HttpPost("SendToArchivum")]
        public async Task<ActionResult> PostDmOrdersSendToArchivum([FromBody] SendToArchivumParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                await _orderProductService.ManagingStatusesForArchive(parameters.OrderId);
                db.EXPOSE_dm_Orders_SendToArchivum /**/ (parameters.OrderId);
                await _save.SaveAsync();
                return Ok();
            }
        }
        public class SelectFromArchivumParams { public int SearchString { get; set; } }
        [HttpGet("SelectFromArchivum")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Orders_SelectFromArchivumResult>>> PostDmOrdersSelectFromArchivum([FromQuery] SelectFromArchivumParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_Orders_SelectFromArchivumAsync /**/ (parameters.SearchString);
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
            using (SPToCoreContext db = new SPToCoreContext())
            {
                int? orderId = 0;
                var findUser = await _userRepository.GetUser(parameters.SenderId);
                parameters.OrderExecutor = "FIORINO Izabela Gądek-Pagacz"; parameters.SourceOfOrder = "Utworzone ręcznie"; parameters.Is_InMagazyn = false;
                parameters.SenderName = findUser.FirstName + " " + findUser.LastName;
                db.EXPOSE_dm_Orders_CreateOrder /**/ (parameters.CreatedAt, parameters.UpdatedAt, parameters.OrderStatusId, parameters.Is_InMagazyn, parameters.SourceOfOrder,
                parameters.OrderExecutor, parameters.SenderName, ref orderId);
                await _save.SaveAsync();
                return Ok(new Response { Message = "OK", Status = "OrderId = " + orderId.ToString() });
            }
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
