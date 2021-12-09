using FioRino_NewProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static FioRino_NewProject.Model.SPToCoreContext;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DmPositionsController : ControllerBase
    {
        [HttpPost("List")]
        public async Task<ActionResult<List<EXPOSE_dm_Positions_ListResult>>> PostDmPositionsList()
        {
            using (SPToCoreContext db = new SPToCoreContext())
            {
                return await db.EXPOSE_dm_Positions_ListAsync /**/ ();

            }
        }
    }
}
