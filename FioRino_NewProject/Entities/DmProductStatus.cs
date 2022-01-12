using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmProductStatus
    {
        public DmProductStatus()
        {
            DmOrderProducts = new HashSet<DmOrderProduct>();
        }

        public int Id { get; set; }
        public string StatusDescription { get; set; }
        public string StatusColor { get; set; }

        public virtual ICollection<DmOrderProduct> DmOrderProducts { get; set; }
    }
}
