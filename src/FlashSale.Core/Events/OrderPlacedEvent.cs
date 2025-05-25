namespace FlashSale.Core.Events
{
    public class OrderPlacedEvent : DomainEvent
    {
        public Order Order { get; }

        public OrderPlacedEvent(Order order)
        {
            Order = order;
        }
    }
}
