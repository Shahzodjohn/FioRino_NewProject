
using FioRino_NewProject.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
