using FioRino_NewProject.DataTransferObjects;
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
    public class DmWzMagazynController : ControllerBase
    {
        [HttpPost("UpdateProductAmountInStanMagazynu")]
        public async Task<ActionResult> PostDmWzMagazynUpdateProductAmountInStanMagazynu([FromBody] UpdateProductAmountInStanMagazynuParams parameters)
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                db.EXPOSE_dm_WzMagazyn_UpdateProductAmountInStanMagazynu /**/ (parameters.WzMagazynId, parameters.Amount);
                return Ok(); 
            }
        }
    }
}
