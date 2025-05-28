namespace FlashSale.Workers.Consumers
{
    public class OrderPlacedConsumer : IConsumer<IOrderPlaced>
    {
        private readonly IEmailService _emailService;

        public OrderPlacedConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<IOrderPlaced> context)
        {
            var msg = context.Message;

            //await _emailService.SendAsync(new EmailDto
            //{
            //    To = msg.Email,
            //    Subject = "Order Confirmation",
            //    Body = $"Your order {msg.OrderId} was placed successfully at {msg.PlacedAt}."
            //});

            Console.WriteLine($" Order Received: {msg.OrderId}");

        }
    }
}
