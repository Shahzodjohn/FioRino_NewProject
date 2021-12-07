using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class OutgoingMessage
    {
        public int Id { get; set; }
        public string Destination { get; set; }
        public string Payload { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Noauto { get; set; }
        public string Comment { get; set; }
    }
}
