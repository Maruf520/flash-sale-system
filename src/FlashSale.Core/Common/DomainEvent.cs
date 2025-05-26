using MediatR;

namespace FlashSale.Core.Common
{
    [NotMapped]
    public abstract class DomainEvent : INotification
    {
        public DateTime OccurredOn { get; protected set; }

        protected DomainEvent()
        {
            OccurredOn = DateTime.UtcNow;
        }
    }
}
