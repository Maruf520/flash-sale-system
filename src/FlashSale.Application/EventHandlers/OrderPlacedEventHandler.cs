namespace FlashSale.Application.EventHandlers
{
    public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEvent>
    {
        private readonly ILogger<OrderPlacedEventHandler> _logger;

        public OrderPlacedEventHandler(ILogger<OrderPlacedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
        {
            var order = notification.Order;

            try
            {
                _logger.LogInformation(
                    "Order placed successfully. OrderId: {OrderId}, UserId: {UserId}, ProductId: {ProductId}, Price: {Price}",
                    order.Id, order.UserId, order.ProductId, order.Price);

                // 🔥 ADD YOUR BUSINESS LOGIC HERE
                // TODO: Publish to RabbitMQ
                // TODO: Send welcome email
                // TODO: Update analytics
                // TODO: Notify inventory system

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
