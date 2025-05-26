namespace FlashSale.Core.Events
{
    public class SaleEndedEvent : DomainEvent
    {
        public FlashSaleEventEntity FlashSaleEvent { get; }

        public SaleEndedEvent(FlashSaleEventEntity flashSaleEvent)
        {
            FlashSaleEvent = flashSaleEvent ?? throw new ArgumentNullException(nameof(flashSaleEvent));
        }
    }
}
