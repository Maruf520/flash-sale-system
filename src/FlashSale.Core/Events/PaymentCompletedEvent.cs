namespace FlashSale.Core.Events
{
    public class PaymentCompletedEvent : DomainEvent
    {
        public Order Order { get; }
        public string PaymentId { get; }
        public decimal Amount { get; }

        public PaymentCompletedEvent(Order order, string paymentId, decimal amount)
        {
            Order = order;
            PaymentId = paymentId;
            Amount = amount;
        }
    }
}
