using MediatR;

namespace FlashSale.Application.Notifications
{
    public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEventNotification>
    {
        public Task Handle(OrderPlacedEventNotification notification, CancellationToken cancellationToken)
        {
            var order = notification.Order;

            Console.WriteLine($"[Domain Event] Order placed: {order.Id}, User: {order.UserId}, Price: {order.Price}");

            // TODO: Publish to RabbitMQ, Send Email, etc.

            return Task.CompletedTask;
        }
    }
}
