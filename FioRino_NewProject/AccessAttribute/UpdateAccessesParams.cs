﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.AccessAttribute
{
    public class UpdateAccessesParams
    {
        public int UserId { get; set; }
        public bool Hurt { get; set; }
        public bool Magazyn { get; set; }
        public bool Archive { get; set; }
    }
}
