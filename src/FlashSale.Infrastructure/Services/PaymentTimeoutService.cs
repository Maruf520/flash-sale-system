namespace FlashSale.Infrastructure.Services
{
    public class PaymentTimeoutService : BackgroundService
    {
        private readonly FlashSaleDbContext _context;
        private readonly ILogger<PaymentTimeoutService> _logger;
        private readonly TimeSpan _paymentTimeout = TimeSpan.FromMinutes(15); // Example timeout

        public PaymentTimeoutService(FlashSaleDbContext context, ILogger<PaymentTimeoutService> logger)
        {
            _context = context;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PaymentTimeoutService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var timeoutThreshold = DateTime.UtcNow - _paymentTimeout;

                    var expiredOrders = await _context.Orders
                        .Include(o => o.Product)
                        .Where(o => o.Status == OrderStatus.Pending && o.CreatedAt < timeoutThreshold)
                        .ToListAsync(stoppingToken);

                    foreach (var order in expiredOrders)
                    {
                        order.Status = OrderStatus.Cancelled;
                        // TODO: Release stock back to inventory/redis here or publish event

                        _logger.LogInformation($"Order {order.Id} canceled due to payment timeout.");
                    }

                    await _context.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in PaymentTimeoutService.");
                }

                // Wait some time before checking again (e.g., every 1 minute)
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("PaymentTimeoutService stopped.");
        }
    }
}
