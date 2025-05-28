namespace FlashSale.Application.Dtos
{
    public class PaymentDto
    {
        public Guid OrderId { get; set; }
        public string PaymentId { get; set; } = default!;
        public string PaymentMethod { get; set; } = default!;
        public decimal Amount { get; set; }
        public string? PaymentGatewayResponse { get; set; }
        public string? TransactionReference { get; set; }
    }
}
