using FlashSale.Core.Events;

namespace FlashSale.Core.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid FlashSaleItemId { get; set; }
        public decimal Price { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime ExpireAt { get; set; }

        public ApplicationUser User { get; set; } = default!;
        public Product Product { get; set; } = default!;
        public FlashSaleItem FlashSaleItem { get; set; } = default!;

        public string? PaymentMethod { get; set; }
        public string? TransactionId { get; set; }

        public static Order Create(
          Guid userId,
          Guid productId,
          Guid flashSaleItemId,
          decimal price,
          DateTime expireAt,
          string? paymentMethod = null,
          string? transactionId = null)
        {
            var order = new Order
            {
                UserId = userId,
                ProductId = productId,
                FlashSaleItemId = flashSaleItemId,
                Price = price,
                Status = OrderStatus.Pending,
                ExpireAt = expireAt,
                PaymentMethod = paymentMethod,
                TransactionId = transactionId
            };

            order.AddDomainEvent(new OrderPlacedEvent(order));

            return order;
        }
    }
}
