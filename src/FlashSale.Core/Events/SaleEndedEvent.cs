namespace FlashSale.Core.Events
{
    public class SaleEndedEvent : DomainEvent
    {
        public FlashSaleEntity FlashSale { get; }

        public SaleEndedEvent(FlashSaleEntity flashSale)
        {
            FlashSale = flashSale ?? throw new ArgumentNullException(nameof(flashSale));
        }
    }
}
