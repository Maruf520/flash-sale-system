namespace FlashSale.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly FlashSaleDbContext _context;

        public ProductRepository(FlashSaleDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.FlashSaleItems)                    
                    .ThenInclude(fsi => fsi.FlashSaleEventEntity)  
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.FlashSaleItems)                    
                    .ThenInclude(fsi => fsi.FlashSaleEventEntity)  
                .ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<IEnumerable<Product>> GetProductsWithActiveFlashSalesAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Products
                .Include(p => p.FlashSaleItems)
                    .ThenInclude(fsi => fsi.FlashSaleEventEntity)
                .Where(p => p.FlashSaleItems.Any(fsi =>
                    fsi.FlashSaleEventEntity.IsActive &&
                    fsi.FlashSaleEventEntity.StartTime <= now &&
                    fsi.FlashSaleEventEntity.EndTime >= now &&
                    fsi.AvailableStock > 0))
                .ToListAsync();
        }

        public async Task<Product?> GetProductWithFlashSaleDetailsAsync(Guid productId)
        {
            return await _context.Products
                .Include(p => p.FlashSaleItems)
                    .ThenInclude(fsi => fsi.FlashSaleEventEntity)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<IEnumerable<Product>> GetProductsByFlashSaleEventAsync(Guid flashSaleEventId)
        {
            return await _context.Products
                .Include(p => p.FlashSaleItems)
                    .ThenInclude(fsi => fsi.FlashSaleEventEntity)
                .Where(p => p.FlashSaleItems.Any(fsi => fsi.FlashSaleEventId == flashSaleEventId))
                .ToListAsync();
        }
    }
}
