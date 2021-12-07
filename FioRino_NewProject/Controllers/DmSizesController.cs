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
    public class DmSizesController : ControllerBase
    {
        [HttpPost("list")]
        public async Task<ActionResult<List<SPToCoreContext.EXPOSE_dm_Sizes_listResult>>> PostDmSizeslist()
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_Sizes_listAsync /**/ ();

            }
        }
    }
}
