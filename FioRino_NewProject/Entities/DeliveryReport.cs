using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DeliveryReport
    {
        public int Id { get; set; }
        public byte Result { get; set; }
        public string MessageId { get; set; }
        public DateTime ProcessDate { get; set; }
    }
}
