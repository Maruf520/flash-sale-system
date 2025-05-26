namespace FlashSale.Core.Enums
{
    public enum OrderStatus
    {
        Pending = 1,
        Paid = 2,
        Expired = 3,
        Cancelled = 4,
        PaymentConfirmedStockFailed = 5,  
        Refunded = 6,
        Shipped = 7,
        Delivered = 8
    }
}
