namespace FlashSale.Core.Repositories
{
    public interface IFlashSaleRepository
    {
        Task<FlashSaleEventEntity?> GetByIdAsync(Guid id);
        Task<FlashSaleEventEntity?> GetActiveFlashSaleEventAsync(Guid eventId);
        Task<List<FlashSaleEventEntity>> GetActiveFlashSaleEventsAsync();
        Task AddAsync(FlashSaleEventEntity flashSaleEvent);
        Task UpdateAsync(FlashSaleEventEntity flashSaleEvent);
        Task DeleteAsync(Guid id);
        Task<FlashSaleItem?> GetFlashSaleItemByProductAndEventAsync(Guid productId, Guid flashSaleEventId);

        Task<FlashSaleItem?> GetFlashSaleItemByIdAsync(Guid itemId);
        Task<FlashSaleItem?> GetActiveFlashSaleItemForProductAsync(Guid productId);
        Task<List<FlashSaleItem>> GetFlashSaleItemsByEventAsync(Guid eventId);
        Task AddFlashSaleItemAsync(FlashSaleItem flashSaleItem);
        Task UpdateFlashSaleItemAsync(FlashSaleItem flashSaleItem);
    }
}
