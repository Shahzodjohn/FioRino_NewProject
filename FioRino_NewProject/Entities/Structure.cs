#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class Structure
    {
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public bool? IsNullable { get; set; }
    }
}
