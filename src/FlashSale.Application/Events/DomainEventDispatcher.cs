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
                await _mediator.Publish(domainEvent);
            }

            entity.ClearDomainEvents();
        }

        public async Task DispatchAndClearEventsAsync(IEnumerable<BaseEntity> entities)
        {
            foreach (var entity in entities)
            {
                await DispatchAndClearEventsAsync(entity);
            }
        }
    }
}
