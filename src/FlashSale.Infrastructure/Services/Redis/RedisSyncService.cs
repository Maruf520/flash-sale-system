using FlashSale.Core.Services;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace FlashSale.Infrastructure.Services.Redis
{
    public class RedisSyncService : IRedisSyncService
    {
        private readonly IConnectionMultiplexer _redis;   
        private readonly IDatabase _database;
        private readonly IFlashSaleRepository _flashSaleRepository;
        private readonly IRedisStockService _redisStockService;
        private readonly ILogger<RedisSyncService> _logger;

        public RedisSyncService(
            IConnectionMultiplexer redis,
            IFlashSaleRepository flashSaleRepository,
            IRedisStockService redisStockService,
            ILogger<RedisSyncService> logger)
        {
            _redis = redis;

            _database = redis.GetDatabase(); 
            _flashSaleRepository = flashSaleRepository;
            _redisStockService = redisStockService;
            _logger = logger;
        }

        public async Task SyncFlashSaleItemToRedisAsync(Guid flashSaleItemId)
        {
            try
            {
                _logger.LogInformation("Syncing FlashSaleItem {FlashSaleItemId} to Redis", flashSaleItemId);

                var flashSaleItem = await _flashSaleRepository.GetFlashSaleItemByIdAsync(flashSaleItemId);
                if (flashSaleItem == null)
                {
                    _logger.LogWarning("FlashSaleItem {FlashSaleItemId} not found in database", flashSaleItemId);
                    return;
                }

                // 🔥 INITIALIZE REDIS FROM DATABASE AVAILABLESTOCK
                await _redisStockService.InitializeStockAsync(
                    flashSaleItemId,
                    totalQuantity: flashSaleItem.AvailableStock,  // Use AvailableStock as initial total
                    soldQuantity: 0,                              // Start with 0 sold
                    availableQuantity: flashSaleItem.AvailableStock // All stock is available initially
                );

                _logger.LogInformation(
                    "FlashSaleItem {FlashSaleItemId} synced to Redis. AvailableStock: {AvailableStock}",
                    flashSaleItemId, flashSaleItem.AvailableStock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync FlashSaleItem {FlashSaleItemId} to Redis", flashSaleItemId);
                throw;
            }
        }

        public async Task SyncAllActiveFlashSaleItemsAsync()
        {
            try
            {
                _logger.LogInformation("Starting sync of all active flash sale items to Redis");

                var activeFlashSaleItems = await _flashSaleRepository.GetActiveFlashSaleItemsAsync();

                var syncTasks = activeFlashSaleItems.Select(async item =>
                {
                    try
                    {
                        await SyncFlashSaleItemToRedisAsync(item.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to sync FlashSaleItem {FlashSaleItemId}", item.Id);
                    }
                });

                await Task.WhenAll(syncTasks);

                _logger.LogInformation(
                    "Completed sync of {Count} active flash sale items to Redis",
                    activeFlashSaleItems.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync active flash sale items to Redis");
                throw;
            }
        }

        public async Task RemoveFlashSaleItemFromRedisAsync(Guid flashSaleItemId)
        {
            try
            {
                await _redisStockService.RemoveStockAsync(flashSaleItemId);
                _logger.LogInformation("FlashSaleItem {FlashSaleItemId} removed from Redis", flashSaleItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove FlashSaleItem {FlashSaleItemId} from Redis", flashSaleItemId);
                // Don't throw - removal failures are not critical
            }
        }

        public async Task<bool> IsFlashSaleItemSyncedAsync(Guid flashSaleItemId)
        {
            try
            {
                return await _redisStockService.ExistsAsync(flashSaleItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check if FlashSaleItem {FlashSaleItemId} is synced", flashSaleItemId);
                return false; // Assume not synced if we can't check
            }
        }

        public async Task UpdateRedisStockFromDatabaseAsync(Guid flashSaleItemId)
        {
            try
            {
                _logger.LogInformation("Updating Redis stock from database for FlashSaleItem {FlashSaleItemId}", flashSaleItemId);

                var flashSaleItem = await _flashSaleRepository.GetFlashSaleItemByIdAsync(flashSaleItemId);
                if (flashSaleItem == null)
                {
                    _logger.LogWarning("FlashSaleItem {FlashSaleItemId} not found in database", flashSaleItemId);
                    return;
                }

                // Get current Redis state
                var currentAvailable = await _redisStockService.GetAvailableStockAsync(flashSaleItemId);
                var currentReserved = await _redisStockService.GetReservedStockAsync(flashSaleItemId);
                var currentSold = await _redisStockService.GetSoldStockAsync(flashSaleItemId);

                // Calculate new totals based on database
                var newTotalQuantity = flashSaleItem.AvailableStock + currentSold; // Database stock + already sold
                var newAvailableQuantity = flashSaleItem.AvailableStock; // What's currently in database

                // Re-sync with current state preserved
                await _redisStockService.InitializeStockAsync(
                    flashSaleItemId,
                    totalQuantity: newTotalQuantity,
                    soldQuantity: currentSold,      // Preserve sold count
                    availableQuantity: newAvailableQuantity
                );

                // Restore reservations if any
                if (currentReserved > 0)
                {
                    await _redisStockService.ReserveStockAsync(flashSaleItemId, currentReserved, 15);
                }

                _logger.LogInformation(
                    "Redis stock updated for FlashSaleItem {FlashSaleItemId}. " +
                    "DB AvailableStock: {DbStock}, Redis Available: {RedisAvailable}, Reserved: {Reserved}, Sold: {Sold}",
                    flashSaleItemId, flashSaleItem.AvailableStock, newAvailableQuantity, currentReserved, currentSold);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Redis stock from database for FlashSaleItem {FlashSaleItemId}", flashSaleItemId);
                throw;
            }
        }
        public async Task<int> GetReservedStockAsync(Guid flashSaleItemId)
        {
            var key = $"stock:{flashSaleItemId}";
            try
            {
                var reserved = await _database.HashGetAsync(key, "reserved");
                return reserved.HasValue ? (int)reserved : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get reserved stock for {FlashSaleItemId}", flashSaleItemId);
                return 0;
            }
        }
        public async Task<int> GetSoldStockAsync(Guid flashSaleItemId)
        {
            var key = $"stock:{flashSaleItemId}";
            try
            {
                var sold = await _database.HashGetAsync(key, "sold");
                return sold.HasValue ? (int)sold : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get sold stock for {FlashSaleItemId}", flashSaleItemId);
                return 0;
            }
        }
    }
}

