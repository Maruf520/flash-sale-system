namespace FlashSale.Core.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid FlashSaleId { get; set; }
        public decimal Price { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime ExpireAt { get; set; }

        public ApplicationUser User { get; set; } = default!;
        public Product Product { get; set; } = default!;
        public FlashDeal FlashSale { get; set; } = default!;
    }
}
