namespace FlashSale.Core.Events
{
    public class OrderCancelledEvent : DomainEvent
    {
        public Order Order { get; }
        public string Reason { get; }

        public OrderCancelledEvent(Order order, string reason)
        {
            Order = order;
            Reason = reason;
        }
    }
}
