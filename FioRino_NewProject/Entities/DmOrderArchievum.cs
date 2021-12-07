using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmOrderArchievum
    {
        public DmOrderArchievum()
        {
            DmOrders = new HashSet<DmOrder>();
        }

        public int Id { get; set; }
        public DateTime CreateAt { get; set; }
        public int OrderStatusId { get; set; }
        public int? FileWzId { get; set; }
        public int Amount { get; set; }
        public int? SenderId { get; set; }
        public int? RecieverId { get; set; }
        public DateTime? ImplementationDate { get; set; }
        public int? OrderProductId { get; set; }

        public virtual DmFileWz FileWz { get; set; }
        public virtual DmOrderStatus OrderStatus { get; set; }
        public virtual DmUser Reciever { get; set; }
        public virtual DmUser Sender { get; set; }
        public virtual ICollection<DmOrder> DmOrders { get; set; }
    }
}
