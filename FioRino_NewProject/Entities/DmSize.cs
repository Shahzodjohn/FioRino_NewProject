using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmSize
    {
        public DmSize()
        {
            DmOrderProducts = new HashSet<DmOrderProduct>();
            DmProducts = new HashSet<DmProduct>();
            DmStorages = new HashSet<DmStorage>();
        }

        public int Id { get; set; }
        public int? Number { get; set; }
        public string Title { get; set; }

        public virtual ICollection<DmOrderProduct> DmOrderProducts { get; set; }
        public virtual ICollection<DmProduct> DmProducts { get; set; }
        public virtual ICollection<DmStorage> DmStorages { get; set; }
    }
}
