using System;

namespace FioRino_NewProject.DataTransferObjects
{
    public class OrderDetailsUpdate
    {
        public int OrderId { get; set; }
        public int SenderId { get; set; }
        public string OrderExecutor { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
