namespace FlashSale.Core.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);


        Task<IEnumerable<Order>> GetByFlashSaleEventIdAsync(Guid flashSaleEventId);
        Task<IEnumerable<Order>> GetPendingOrdersAsync();
        Task<IEnumerable<Order>> GetExpiredOrdersAsync();
    }
}
