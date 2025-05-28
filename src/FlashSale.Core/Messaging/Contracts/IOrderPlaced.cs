namespace FlashSale.Core.Messaging.Contracts
{
    public interface IOrderPlaced
    {
        Guid OrderId { get; }
        string Email { get; }
        DateTime PlacedAt { get; }
    }
}
