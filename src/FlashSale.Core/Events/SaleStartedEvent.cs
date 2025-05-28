namespace FlashSale.Core.Events
{
    public class SaleStartedEvent
    {
        public FlashSaleEventEntity FlashSaleEvent { get; }

        public SaleStartedEvent(FlashSaleEventEntity flashSaleEvent)
        {
            FlashSaleEvent = flashSaleEvent ?? throw new ArgumentNullException(nameof(flashSaleEvent));
        }
    }
}
