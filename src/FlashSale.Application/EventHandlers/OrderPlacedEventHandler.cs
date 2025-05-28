namespace FlashSale.Application.EventHandlers
{
    public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEvent>
    {
        private readonly ILogger<OrderPlacedEventHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        public OrderPlacedEventHandler(ILogger<OrderPlacedEventHandler> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
        {
            var order = notification.Order;

            try
            {
                await _publishEndpoint.Publish<IOrderPlaced>(new
                {
                    OrderId = order.Id,

                    PlacedAt = DateTime.UtcNow
                });

                Console.WriteLine($"[Domain Event] Order placed: {order.Id}, User: {order.UserId}, Price: {order.Price}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error processing order placed event for Order {OrderId}",
                    order.Id);
            }
        }
    }
}
