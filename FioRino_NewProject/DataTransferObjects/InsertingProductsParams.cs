namespace FioRino_NewProject.DataTransferObjects
{
    public class InsertingProductsParams
    {
        public int UniqueProductId { get; set; }
        public int SkuCodeId { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int SizeId { get; set; }
        public int Amount { get; set; }
        public int StorageId { get; set; }
    }
}
