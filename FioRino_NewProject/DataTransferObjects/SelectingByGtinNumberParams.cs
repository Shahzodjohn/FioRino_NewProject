using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.DataTransferObjects
{
    public class SelectingByGtinNumberParams
    {
        public string Gtin { get; set; }
        public int Amount { get; set; }
        public int SkuCodeId { get; set; }
    }
}
