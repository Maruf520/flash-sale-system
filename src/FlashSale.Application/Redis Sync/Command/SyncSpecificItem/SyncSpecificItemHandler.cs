public class SyncSpecificItemHandler(
    IRedisSyncService redisSyncService,
    IRedisStockService redisStockService,
    IFlashSaleRepository flashSaleRepository,
    ILogger<SyncSpecificItemHandler> logger
) : IRequestHandler<SyncSpecificItemCommand, SyncSpecificItemResult>
{
    public async Task<SyncSpecificItemResult> Handle(SyncSpecificItemCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation("Sync specific item command triggered for {FlashSaleItemId} by {TriggeredBy}",
            request.FlashSaleItemId, request.TriggeredBy ?? "Unknown");

        try
        {
            // Check if item exists in database
            var dbItem = await flashSaleRepository.GetFlashSaleItemByIdAsync(request.FlashSaleItemId);
            if (dbItem == null)
            {
                stopwatch.Stop();
                var notFoundMessage = $"FlashSaleItem {request.FlashSaleItemId} not found in database";

                logger.LogWarning("Sync failed: FlashSaleItem {FlashSaleItemId} not found in database", request.FlashSaleItemId);

                return new SyncSpecificItemResult(
                    false,
                    notFoundMessage,
                    stopwatch.ElapsedMilliseconds,
                    DateTime.UtcNow,
                    request.FlashSaleItemId,
                    null,
                    null,
                    null,
                    "Item not found in database"
                );
            }

            var productName = dbItem.Product?.Name;
            var databaseStock = dbItem.AvailableStock;

            // Clear Redis if force sync
            if (request.ForceSync)
            {
                await redisStockService.RemoveStockAsync(request.FlashSaleItemId);
            }

            // Perform sync
            await redisSyncService.SyncFlashSaleItemToRedisAsync(request.FlashSaleItemId);

            // Verify sync
            var exists = await redisStockService.ExistsAsync(request.FlashSaleItemId);
            int? redisStock = null;
            if (exists)
            {
                redisStock = await redisStockService.GetAvailableStockAsync(request.FlashSaleItemId);
            }

            stopwatch.Stop();

            var success = exists;
            var message = success
                ? $"FlashSaleItem {request.FlashSaleItemId} synced successfully"
                : $"Sync completed but item not found in Redis";

            logger.LogInformation("Sync specific item completed for {FlashSaleItemId}: {Success}, Duration: {Duration}ms",
                request.FlashSaleItemId, success, stopwatch.ElapsedMilliseconds);

            return new SyncSpecificItemResult(
                success,
                message,
                stopwatch.ElapsedMilliseconds,
                DateTime.UtcNow,
                request.FlashSaleItemId,
                productName,
                databaseStock,
                redisStock,
                success ? null : "Item not found in Redis after sync"
            );
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            logger.LogError(ex, "Sync specific item failed for {FlashSaleItemId} after {Duration}ms",
                request.FlashSaleItemId, stopwatch.ElapsedMilliseconds);

            return new SyncSpecificItemResult(
                false,
                $"Sync failed: {ex.Message}",
                stopwatch.ElapsedMilliseconds,
                DateTime.UtcNow,
                request.FlashSaleItemId,
                null,
                null,
                null,
                ex.ToString()
            );
        }
    }
}
