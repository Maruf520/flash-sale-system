namespace FlashSale.Core.Services
{
    public interface IRedisSyncService
    {
        Task SyncFlashSaleItemToRedisAsync(Guid flashSaleItemId);
        Task SyncAllActiveFlashSaleItemsAsync();
        Task RemoveFlashSaleItemFromRedisAsync(Guid flashSaleItemId);
        Task<bool> IsFlashSaleItemSyncedAsync(Guid flashSaleItemId);
        Task UpdateRedisStockFromDatabaseAsync(Guid flashSaleItemId);
    }
}
