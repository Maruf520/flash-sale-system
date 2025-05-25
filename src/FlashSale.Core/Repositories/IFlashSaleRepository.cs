namespace FlashSale.Core.Repositories
{
    public interface IFlashSaleRepository
    {
        Task<FlashDeal?> GetActiveFlashSaleForProductAsync(Guid productId);
        Task<FlashDeal?> GetByIdAsync(Guid id);
        Task AddAsync(FlashDeal flashSale);
        Task UpdateAsync(FlashDeal flashSale);
    }
}
