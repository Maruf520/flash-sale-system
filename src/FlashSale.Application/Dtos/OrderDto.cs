namespace FlashSale.Application.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid FlashSaleId { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = default!;
        public DateTime ExpireAt { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
    }
}
