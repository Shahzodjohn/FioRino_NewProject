using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmWzMagazyn
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int OrderStatusesId { get; set; }
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }

        public virtual DmOrderStatus OrderStatuses { get; set; }
        public virtual DmProduct Product { get; set; }
    }
}
