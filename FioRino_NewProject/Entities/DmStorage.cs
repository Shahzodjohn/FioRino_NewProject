using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmStorage
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? SizeId { get; set; }
        public int? CategoryId { get; set; }
        public string Gtin { get; set; }
        public int? Amount { get; set; }
        public int? SkuCodeId { get; set; }
        public bool? IsBlocked { get; set; }
        public int? AmountLeft { get; set; }
        public int? UniqueProductId { get; set; }

        public virtual DmCategory Category { get; set; }
        public virtual DmProduct Product { get; set; }
        public virtual DmSize Size { get; set; }
        public virtual DmUniqueProduct UniqueProduct { get; set; }
    }
}
