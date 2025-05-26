namespace FlashSale.Core.Common
{
    [NotMapped]
    public abstract class DomainEvent
    {
        public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
    }
}
