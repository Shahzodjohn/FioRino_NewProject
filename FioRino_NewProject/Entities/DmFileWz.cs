using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmFileWz
    {
        public DmFileWz()
        {
            DmOrderArchievums = new HashSet<DmOrderArchievum>();
        }

        public int Id { get; set; }
        public string FileName { get; set; }
        public int FileType { get; set; }
        public decimal FileSize { get; set; }
        public int UserId { get; set; }

        public virtual DmUser User { get; set; }
        public virtual ICollection<DmOrderArchievum> DmOrderArchievums { get; set; }
    }
}
