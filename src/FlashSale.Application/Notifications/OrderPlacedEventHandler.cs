using MediatR;
using Microsoft.Extensions.Logging;

namespace FlashSale.Application.Notifications
{
    public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEventNotification>
    {
        private readonly ILogger<OrderPlacedEventHandler> _logger;
        public OrderPlacedEventHandler(ILogger<OrderPlacedEventHandler> logger)
        {
            _logger = logger;
        }
        public Task Handle(OrderPlacedEventNotification notification, CancellationToken cancellationToken)
        {
            var order = notification.Order;

            Console.WriteLine($"[Domain Event] Order placed: {order.Id}, User: {order.UserId}, Price: {order.Price}");
            _logger.LogInformation(
    "Order placed successfully. OrderId: {OrderId}, UserId: {UserId}, ProductId: {ProductId}, Price: {Price}",
    order.Id, order.UserId, order.ProductId, order.Price);

            // TODO: Publish to RabbitMQ, Send Email, etc.

            return Task.CompletedTask;
        }
    }
}
