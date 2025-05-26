namespace FlashSale.Core.Entities
{
    public class FlashSaleItem : BaseEntity
    {
        public Guid FlashSaleEventId { get; set; }
        public Guid ProductId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public int AvailableStock { get; set; }
        public FlashSaleEventEntity FlashSaleEventEntity { get; set; } = default!;
        public Product Product { get; set; } = default!;
    }
}
