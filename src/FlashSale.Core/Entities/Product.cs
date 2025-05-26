namespace FlashSale.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal OriginalPrice { get; set; }
        public int TotalStock { get; set; }

        public ICollection<FlashSaleItem> FlashSaleItems { get; set; } = new List<FlashSaleItem>();
    }
}
