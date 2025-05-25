namespace FlashSale.Core.Events
{
    public class SaleStartedEvent
    {
        public FlashDeal FlashSale { get; }

        public SaleStartedEvent(FlashDeal flashSale)
        {
            FlashSale = flashSale;
        }
    }
}
