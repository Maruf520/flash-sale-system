using FlashSale.Core.Common;

namespace FlashSale.Application.Events
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAndClearEventsAsync(BaseEntity entity);
    }
}
