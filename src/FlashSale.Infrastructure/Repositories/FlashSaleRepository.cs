namespace FlashSale.Infrastructure.Repositories
{
    public class FlashSaleRepository : IFlashSaleRepository
    {
        private readonly FlashSaleDbContext _context;

        public FlashSaleRepository(FlashSaleDbContext context)
        {
            _context = context;
        }

        public async Task<FlashDeal?> GetActiveFlashSaleForProductAsync(Guid productId)
        {
            var now = DateTime.UtcNow;
            return await _context.FlashDeals
                .FirstOrDefaultAsync(fd => fd.ProductId == productId &&
                                           fd.StartTime <= now &&
                                           fd.EndTime >= now &&
                                           fd.AvailableStock > 0);
        }

        public async Task<FlashDeal?> GetByIdAsync(Guid id)
        {
            return await _context.FlashDeals
                .FirstOrDefaultAsync(fd => fd.Id == id);
        }

        public async Task AddAsync(FlashDeal flashSale)
        {
            await _context.FlashDeals.AddAsync(flashSale);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FlashDeal flashSale)
        {
            _context.FlashDeals.Update(flashSale);
            await _context.SaveChangesAsync();
        }
    }
}
