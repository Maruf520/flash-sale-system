namespace FlashSale.Infrastructure.Caching
{
    public interface IRedisCacheService
    {
        Task<T?> GetStockAsync<T>(string key);
        Task SetStockAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
    }
}
