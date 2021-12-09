
using FioRino_NewProject.Model;
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
    public class DmSKUCodesController : ControllerBase
    {
        [HttpPost("list")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_SKUCodes_listResult>>> PostDmSKUCodeslist()
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_SKUCodes_listAsync /**/ ();
            }
        }
    }
}
