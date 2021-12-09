using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmOrder
    {
        public DmOrder()
        {
            DmOrderProducts = new HashSet<DmOrderProduct>();
        }

        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? OrderStatusId { get; set; }
        public int? OrderArchievumId { get; set; }
        public string SourceOfOrder { get; set; }
        public bool? IsInMagazyn { get; set; }
        public int? Amount { get; set; }
        public string OrderExecutor { get; set; }
        public int? ReceiverId { get; set; }
        public int? SenderId { get; set; }
        public bool? IsInArchievum { get; set; }
        public bool? IsBlocked { get; set; }
        public DateTime? DateOfRelease { get; set; }

        public virtual DmOrderArchievum OrderArchievum { get; set; }
        public virtual DmUser Receiver { get; set; }
        public virtual DmUser Sender { get; set; }
        public virtual ICollection<DmOrderProduct> DmOrderProducts { get; set; }
    }
}
