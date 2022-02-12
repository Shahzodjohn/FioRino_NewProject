using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.DataTransferObjects
{
    public class ShoperOrdersDTO
    {
        public string count { get; set; }
        public IEnumerable<Object> List { get; set; }
    }
}
    