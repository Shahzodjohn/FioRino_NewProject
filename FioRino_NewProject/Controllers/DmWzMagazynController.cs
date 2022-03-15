using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Model;
using Microsoft.AspNetCore.Mvc;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DmWzMagazynController : ControllerBase
    {
        [HttpPost("UpdateProductAmountInStanMagazynu")]
        public ActionResult PostDmWzMagazynUpdateProductAmountInStanMagazynu([FromBody] UpdateProductAmountInStanMagazynuParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                 db.EXPOSE_dm_WzMagazyn_UpdateProductAmountInStanMagazynu /**/ (parameters.WzMagazynId, parameters.Amount);
                return Ok();
            }
        }
    }
}
