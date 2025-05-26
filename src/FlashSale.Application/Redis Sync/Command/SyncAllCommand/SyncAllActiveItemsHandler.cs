using FlashSale.Core.Repositories;
using FlashSale.Core.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

public class SyncAllActiveItemsHandler(
    IRedisSyncService redisSyncService,
    IRedisStockService redisStockService,
    IFlashSaleRepository flashSaleRepository,
    ILogger<SyncAllActiveItemsHandler> logger
) : IRequestHandler<SyncAllActiveItemsCommand, SyncAllActiveItemsResult>
{
    public async Task<SyncAllActiveItemsResult> Handle(SyncAllActiveItemsCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var errors = new List<string>();

        logger.LogInformation("Sync all active items command triggered by {TriggeredBy}, ForceSync: {ForceSync}",
            request.TriggeredBy ?? "Unknown", request.ForceSync);

        try
        {
            // Get all active items for counting
            var activeItems = await flashSaleRepository.GetActiveFlashSaleItemsAsync();
            var itemsProcessed = activeItems.Count();

            if (request.ForceSync)
            {
                logger.LogInformation("Force sync requested - clearing Redis data first");

                // Clear existing Redis data
                var clearTasks = activeItems.Select(async item =>
                {
                    try
                    {
                        await redisStockService.RemoveStockAsync(item.Id);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to clear Redis data for item {ItemId}", item.Id);
                        errors.Add($"Clear failed for {item.Id}: {ex.Message}");
                    }
                });
                await Task.WhenAll(clearTasks);
            }

            // Perform the sync
            await redisSyncService.SyncAllActiveFlashSaleItemsAsync();

            // Verify sync results
            var successCount = 0;
            var failCount = 0;

            foreach (var item in activeItems)
            {
                try
                {
                    var exists = await redisStockService.ExistsAsync(item.Id);
                    if (exists)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        errors.Add($"Item {item.Id} not found in Redis after sync");
                    }
                }
                catch (Exception ex)
                {
                    failCount++;
                    errors.Add($"Verification failed for {item.Id}: {ex.Message}");
                }
            }

            stopwatch.Stop();

            var success = failCount == 0;
            var message = success
                ? $"Successfully synced {successCount} items"
                : $"Sync completed with errors: {successCount} successful, {failCount} failed";

            logger.LogInformation("Sync all active items completed: {Success}, Duration: {Duration}ms, Success: {SuccessCount}, Failed: {FailCount}",
                success, stopwatch.ElapsedMilliseconds, successCount, failCount);

            return new SyncAllActiveItemsResult(
                success,
                message,
                stopwatch.ElapsedMilliseconds,
                DateTime.UtcNow,
                itemsProcessed,
                successCount,
                failCount,
                errors
            );
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            errors.Add(ex.ToString());

            logger.LogError(ex, "Sync all active items failed after {Duration}ms", stopwatch.ElapsedMilliseconds);

            return new SyncAllActiveItemsResult(
                false,
                $"Sync failed: {ex.Message}",
                stopwatch.ElapsedMilliseconds,
                DateTime.UtcNow,
                0,
                0,
                0,
                errors
            );
        }
    }
}
