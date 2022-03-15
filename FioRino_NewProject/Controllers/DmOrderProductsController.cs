using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Model;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DmOrderProductsController : ControllerBase
    {
        private readonly IOrderProductsService _opService;

        public DmOrderProductsController(IOrderProductsService opService)
        {
            _opService = opService;
        }

        [HttpPost("InsertProduct")]
        public async Task<IActionResult> PostDmOrderProductsInsertProduct([FromBody] InsertProductParams parameters)
        {
            var responseMessage = await _opService.PostDmOrderProductsInsertProduct(parameters);
            if (responseMessage.Status == "Ok")
            {
                return Ok(new Response { Status = responseMessage.Status, Message = responseMessage.Message });
            }
            else
                return BadRequest(new Response { Status = responseMessage.Status, Message = responseMessage.Message });
        }
        [HttpDelete("DeleteRangeById")]
        public async Task<IActionResult> DeleteDmOrderProducts(List<int> Ids)
        {
            var responseMessage = await _opService.DeleteDmOrderProducts(Ids);
            if (responseMessage.Status == "Ok")
            {
                return Ok(new Response { Status = responseMessage.Status, Message = responseMessage.Message });
            }
            else
                return BadRequest(new Response { Status = responseMessage.Status, Message = responseMessage.Message });
        }
        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProducts([FromBody] OrderProductDTO dTO)
        {
            var responseMessage = await _opService.UpdateProducts(dTO);
            if (responseMessage.Status == "Ok")
            {
                return Ok(new Response { Status = responseMessage.Status, Message = responseMessage.Message });
            }
            else
                return BadRequest(new Response { Status = responseMessage.Status, Message = responseMessage.Message });
        }
        public class GetOrderDetailParams { public int OrderId { get; set; } }
        [HttpPost("GetOrderDetail")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_OrderProducts_GetOrderDetailResult>>> PostDmOrderProductsGetOrderDetail([FromBody] GetOrderDetailParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_OrderProducts_GetOrderDetailAsync /**/ (parameters.OrderId);
            }
        }
    }
}
