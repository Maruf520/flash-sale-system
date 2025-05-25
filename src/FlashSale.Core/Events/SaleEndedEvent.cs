namespace FlashSale.Core.Events
{
    public class SaleEndedEvent : DomainEvent
    {
        public FlashDeal FlashSale { get; }

        public SaleEndedEvent(FlashDeal flashSale)
        {
            FlashSale = flashSale ?? throw new ArgumentNullException(nameof(flashSale));
        }
    }
}
