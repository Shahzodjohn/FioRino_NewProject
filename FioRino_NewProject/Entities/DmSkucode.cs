using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmSkucode
    {
        public DmSkucode()
        {
            DmOrderProducts = new HashSet<DmOrderProduct>();
            DmUniqueProducts = new HashSet<DmUniqueProduct>();
        }

        public int Id { get; set; }
        public string SkucodeName { get; set; }

        public virtual ICollection<DmOrderProduct> DmOrderProducts { get; set; }
        public virtual ICollection<DmUniqueProduct> DmUniqueProducts { get; set; }
    }
}
