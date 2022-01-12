using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmCategory
    {
        public DmCategory()
        {
            DmOrderProducts = new HashSet<DmOrderProduct>();
            DmProducts = new HashSet<DmProduct>();
            DmStorages = new HashSet<DmStorage>();
        }

        public int Id { get; set; }
        public string CategoryName { get; set; }

        public virtual ICollection<DmOrderProduct> DmOrderProducts { get; set; }
        public virtual ICollection<DmProduct> DmProducts { get; set; }
        public virtual ICollection<DmStorage> DmStorages { get; set; }
    }
}
