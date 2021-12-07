using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.DataTransferObjects
{
    public class ByOrderIdParams
    {
        public int OrderId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
