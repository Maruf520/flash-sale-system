namespace FlashSale.Core.Repositories
{
    public interface IInventoryRepository
    {
        Task<int> GetAvailableStockAsync(Guid productId);
        Task<bool> ReserveStockAsync(Guid productId, int quantity);
        Task<bool> ReleaseStockAsync(Guid productId, int quantity);
    }
}
