using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.DataTransferObjects
{
    public class InsertProductParams
    {
        public int ProductId { get; set; }
        public int UniqueProductId { get; set; }
        public int OrderId { get; set; }
        public int SizeId { get; set; }
        public int SKUcodeId { get; set; }
        public int CategoryId { get; set; }
        public int Amount { get; set; }
        public int OrderProductId { get; set; }
    }
}
