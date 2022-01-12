namespace FioRino_NewProject.DataTransferObjects
{
    public class OrderProductDTO
    {
        public int Id { get; set; }
        public int UniqueProductId { get; set; }
        public int ProductId { get; set; }
        public int? SizeId { get; set; }
        public int? SkUcodeId { get; set; }
        public int? CategoryId { get; set; }
        public int? Amount { get; set; }
    }
}
