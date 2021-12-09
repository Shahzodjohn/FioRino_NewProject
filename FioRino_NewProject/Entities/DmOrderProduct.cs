using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmOrderProduct
    {
        public DmOrderProduct()
        {
            DmWzMagazyns = new HashSet<DmWzMagazyn>();
        }

        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int OrderId { get; set; }
        public int? Amount { get; set; }
        public int? SizeId { get; set; }
        public int? SkucodeId { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductStatusesId { get; set; }
        public int? UniqueProductId { get; set; }
        public string Gtin { get; set; }

        public virtual DmCategory Category { get; set; }
        public virtual DmOrder Order { get; set; }
        public virtual DmProduct Product { get; set; }
        public virtual DmProductStatus ProductStatuses { get; set; }
        public virtual DmSize Size { get; set; }
        public virtual DmSkucode Skucode { get; set; }
        public virtual DmUniqueProduct UniqueProduct { get; set; }
        public virtual ICollection<DmWzMagazyn> DmWzMagazyns { get; set; }
    }
}
