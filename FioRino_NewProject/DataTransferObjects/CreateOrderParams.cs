using System;

namespace FioRino_NewProject.DataTransferObjects
{
    public class CreateOrderParams
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int OrderStatusId { get; set; }
        public bool Is_InMagazyn { get; set; }
        public string SourceOfOrder { get; set; }
        public string OrderExecutor { get; set; }
        public string SenderName { get; set; }
        public int OrderId { get; set; }
        public int SenderId { get; set; }

    }
}
