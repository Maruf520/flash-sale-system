namespace FlashSale.Core.Entities
{
    public class FlashSale : BaseEntity
    {
        public Guid ProductId { get; set; }
        public decimal DiscountedPrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int AvailableStock { get; set; }

        public Product Product { get; set; } = default!;
    }
}
