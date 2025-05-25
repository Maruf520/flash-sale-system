namespace FlashSale.Core.Events
{
    public class SaleStartedEvent
    {
        public FlashSaleEntity FlashSale { get; }

        public SaleStartedEvent(FlashSaleEntity flashSale)
        {
            FlashSale = flashSale;
        }
    }
}
