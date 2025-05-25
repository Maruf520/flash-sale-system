namespace FlashSale.Core.Events
{
    public class InventoryUpdatedEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public int NewStock { get; }

        public InventoryUpdatedEvent(Guid productId, int newStock)
        {
            ProductId = productId;
            NewStock = newStock;
        }
    }
}
