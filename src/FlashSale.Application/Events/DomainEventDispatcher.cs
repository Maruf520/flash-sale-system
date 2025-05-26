using FlashSale.Application.Notifications;
using FlashSale.Core.Common;
using FlashSale.Core.Events;
using MediatR;

namespace FlashSale.Application.Events
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task DispatchAndClearEventsAsync(BaseEntity entity)
        {
            var domainEvents = entity.DomainEvents.ToList();

            foreach (var domainEvent in domainEvents)
            {
                switch (domainEvent)
                {
                    case OrderPlacedEvent e:
                        await _mediator.Publish(new OrderPlacedEventNotification(e.Order));
                        break;

                        // Add more event mappings here if needed
                }
            }

            entity.ClearDomainEvents();
        }
    }
}
