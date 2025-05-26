namespace FlashSale.Core.Services
{
    public interface IRedisStockService
    {
        Task InitializeStockAsync(Guid flashSaleItemId, int totalQuantity, int soldQuantity, int availableQuantity);
        Task RemoveStockAsync(Guid flashSaleItemId);
        Task<bool> ExistsAsync(Guid flashSaleItemId);
        Task<bool> ReserveStockAsync(Guid flashSaleItemId, int quantity = 1, int reservationTtlMinutes = 15);
        Task<bool> ConfirmStockReservationAsync(Guid flashSaleItemId, int quantity = 1);
        Task<bool> ReleaseStockAsync(Guid flashSaleItemId, int quantity = 1);
        Task<int> GetAvailableStockAsync(Guid flashSaleItemId);
        Task InitializeStockAsync(Guid flashSaleItemId, int initialStock);
        Task<bool> IsStockAvailableAsync(Guid flashSaleItemId, int quantity = 1);
        Task<int> GetReservedStockAsync(Guid flashSaleItemId);    
        Task<int> GetSoldStockAsync(Guid flashSaleItemId);      
    }
}
