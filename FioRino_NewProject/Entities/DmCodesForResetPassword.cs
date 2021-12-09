using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmCodesForResetPassword
    {
        public int Id { get; set; }
        public string RandomNumber { get; set; }
        public DateTime? ValidDate { get; set; }
        public int? UserId { get; set; }

        public virtual DmUser User { get; set; }
    }
}
