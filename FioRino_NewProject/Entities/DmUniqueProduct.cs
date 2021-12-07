using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmUniqueProduct
    {
        public DmUniqueProduct()
        {
            DmOrderProducts = new HashSet<DmOrderProduct>();
            DmProducts = new HashSet<DmProduct>();
            DmStorages = new HashSet<DmStorage>();
        }

        public int Id { get; set; }
        public string ProductName { get; set; }

        public virtual ICollection<DmOrderProduct> DmOrderProducts { get; set; }
        public virtual ICollection<DmProduct> DmProducts { get; set; }
        public virtual ICollection<DmStorage> DmStorages { get; set; }
    }
}
