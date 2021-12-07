using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmRole
    {
        public DmRole()
        {
            DmUsers = new HashSet<DmUser>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; }

        public virtual ICollection<DmUser> DmUsers { get; set; }
    }
}
