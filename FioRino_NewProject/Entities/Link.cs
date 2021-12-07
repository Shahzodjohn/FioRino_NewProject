using System;
using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class Link
    {
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string ReferenceTable { get; set; }
        public string ReferenceField { get; set; }
        public string ReferenceEntityType { get; set; }
        public byte? ReferenceType { get; set; }
    }
}
