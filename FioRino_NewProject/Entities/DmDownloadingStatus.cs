using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmDownloadingStatus
    {
        public int Id { get; set; }
        public int? TotalAmount { get; set; }
        public int? CurrentAmount { get; set; }
        public DateTime? SuccessDate { get; set; }
        public string Status { get; set; }
    }
}
