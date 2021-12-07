﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.DataTransferObjects
{
    public class CreateOrderParams
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int OrderStatusId { get; set; }
        public bool Is_InMagazyn { get; set; }
        public int SenderId { get; set; }
        public int Amount { get; set; }
        public string SourceOfOrder { get; set; }
        public string OrderExecutor { get; set; }
        public int OrderId { get; set; }
    }
}
