using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmUsersAccess 
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool? Hurt { get; set; }
        public bool? Magazyn { get; set; }
        public bool? Archive { get; set; }

        public virtual DmUser User { get; set; }
    }
}
