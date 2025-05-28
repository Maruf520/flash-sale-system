namespace FlashSale.Infrastructure.Repositories
{
    internal class InventoryRepository : IInventoryRepository
    {
        private readonly FlashSaleDbContext _context;

        public InventoryRepository(FlashSaleDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetAvailableStockAsync(Guid productId)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);

            return product?.TotalStock ?? 0;
        }

        public async Task<bool> ReserveStockAsync(Guid productId, int quantity)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null || product.TotalStock < quantity)
                return false;

            product.TotalStock -= quantity;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReleaseStockAsync(Guid productId, int quantity)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return false;

            product.TotalStock += quantity;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
