public class SyncAllActiveItemsHandler(
    IRedisSyncService _redisSyncService,
    IRedisStockService _redisStockService,
    IFlashSaleRepository _flashSaleRepository,
    ILogger<SyncAllActiveItemsHandler> _logger
) : IRequestHandler<SyncAllActiveItemsCommand, SyncAllActiveItemsResult>
{
    public async Task<SyncAllActiveItemsResult> Handle(SyncAllActiveItemsCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var errors = new List<string>();

        _logger.LogInformation("Sync all active items command triggered by {TriggeredBy}, ForceSync: {ForceSync}",
            request.TriggeredBy ?? "Unknown", request.ForceSync);

        try
        {
            var activeItems = await _flashSaleRepository.GetActiveFlashSaleItemsAsync();
            var itemsProcessed = activeItems.Count();

            if (request.ForceSync)
            {
                _logger.LogInformation("Force sync requested - clearing Redis data first");

                var clearTasks = activeItems.Select(async item =>
                {
                    try
                    {
                        await _redisStockService.RemoveStockAsync(item.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to clear Redis data for item {ItemId}", item.Id);
                        errors.Add($"Clear failed for {item.Id}: {ex.Message}");
                    }
                });
                await Task.WhenAll(clearTasks);
            }


            await _redisSyncService.SyncAllActiveFlashSaleItemsAsync();

            var successCount = 0;
            var failCount = 0;

            foreach (var item in activeItems)
            {
                try
                {
                    var exists = await _redisStockService.ExistsAsync(item.Id);
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

            _logger.LogInformation("Sync all active items completed: {Success}, Duration: {Duration}ms, Success: {SuccessCount}, Failed: {FailCount}",
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

            _logger.LogError(ex, "Sync all active items failed after {Duration}ms", stopwatch.ElapsedMilliseconds);

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
