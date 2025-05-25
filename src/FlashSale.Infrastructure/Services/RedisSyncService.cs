using FlashSale.Infrastructure.Caching;

namespace FlashSale.Infrastructure.Services
{
    public class RedisSyncService : BackgroundService
    {
        private readonly FlashSaleDbContext _context;
        private readonly IRedisCacheService _redisCacheService;
        private readonly ILogger<RedisSyncService> _logger;

        public RedisSyncService(
            FlashSaleDbContext context,
            IRedisCacheService redisCacheService,
            ILogger<RedisSyncService> logger)
        {
            _context = context;
            _redisCacheService = redisCacheService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RedisSyncService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var products = await _context.Products.ToListAsync(stoppingToken);

                    foreach (var product in products)
                    {
                        await _redisCacheService.SetStockAsync(product.Id.ToString(), product.TotalStock);
                    }

                    _logger.LogInformation("Redis stock synchronization completed.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during Redis stock synchronization.");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _logger.LogInformation("RedisSyncService stopped.");
        }
    }
}
