namespace FlashSale.Core.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);

        Task<IEnumerable<Product>> GetProductsWithActiveFlashSalesAsync();
        Task<Product?> GetProductWithFlashSaleDetailsAsync(Guid productId);
        Task<IEnumerable<Product>> GetProductsByFlashSaleEventAsync(Guid flashSaleEventId);
    }
}
