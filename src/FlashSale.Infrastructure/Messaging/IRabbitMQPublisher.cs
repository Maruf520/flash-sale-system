namespace FlashSale.Infrastructure.Messaging
{
    public interface IRabbitMQPublisher
    {
        void Publish<T>(T message, string queueName);
    }
}
