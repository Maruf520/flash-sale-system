namespace FlashSale.Infrastructure.Repositories
{
    public class FlashSaleRepository : IFlashSaleRepository
    {
        private readonly FlashSaleDbContext _context;

        public FlashSaleRepository(FlashSaleDbContext context)
        {
            _context = context;
        }

        #region FlashSaleEventEntity Methods

        public async Task<FlashSaleEventEntity?> GetByIdAsync(Guid id)
        {
            return await _context.FlashSaleEventEntities
                .Include(fse => fse.FlashSaleItems)
                    .ThenInclude(fsi => fsi.Product)
                .FirstOrDefaultAsync(fse => fse.Id == id);
        }
        public async Task<FlashSaleItem?> GetFlashSaleItemByProductAndEventAsync(Guid productId, Guid flashSaleEventId)
        {
            return await _context.FlashSaleItems
                .Include(fsi => fsi.Product)
                .Include(fsi => fsi.FlashSaleEventEntity)
                .FirstOrDefaultAsync(fsi => fsi.ProductId == productId &&
                                           fsi.FlashSaleEventId == flashSaleEventId);
        }
        public async Task<FlashSaleEventEntity?> GetActiveFlashSaleEventAsync(Guid eventId)
        {
            var now = DateTime.UtcNow;
            return await _context.FlashSaleEventEntities
                .Include(fse => fse.FlashSaleItems)
                    .ThenInclude(fsi => fsi.Product)
                .FirstOrDefaultAsync(fse => fse.Id == eventId &&
                                           fse.StartTime <= now &&
                                           fse.EndTime >= now &&
                                           fse.IsActive);
        }

        public async Task<List<FlashSaleEventEntity>> GetActiveFlashSaleEventsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.FlashSaleEventEntities
                .Include(fse => fse.FlashSaleItems)
                    .ThenInclude(fsi => fsi.Product)
                .Where(fse => fse.StartTime <= now &&
                             fse.EndTime >= now &&
                             fse.IsActive)
                .ToListAsync();
        }

        public async Task AddAsync(FlashSaleEventEntity flashSaleEvent)
        {
            await _context.FlashSaleEventEntities.AddAsync(flashSaleEvent);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FlashSaleEventEntity flashSaleEvent)
        {
            _context.FlashSaleEventEntities.Update(flashSaleEvent);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var flashSaleEvent = await _context.FlashSaleEventEntities.FindAsync(id);
            if (flashSaleEvent != null)
            {
                _context.FlashSaleEventEntities.Remove(flashSaleEvent);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region FlashSaleItem Methods

        public async Task<FlashSaleItem?> GetFlashSaleItemByIdAsync(Guid itemId)
        {
            return await _context.FlashSaleItems
                .Include(fsi => fsi.Product)
                .Include(fsi => fsi.FlashSaleEventEntity)
                .FirstOrDefaultAsync(fsi => fsi.Id == itemId);
        }

        public async Task<FlashSaleItem?> GetActiveFlashSaleItemForProductAsync(Guid productId)
        {
            var now = DateTime.UtcNow;
            return await _context.FlashSaleItems
                .Include(fsi => fsi.Product)
                .Include(fsi => fsi.FlashSaleEventEntity)
                .FirstOrDefaultAsync(fsi => fsi.ProductId == productId &&
                                           fsi.FlashSaleEventEntity.StartTime <= now &&
                                           fsi.FlashSaleEventEntity.EndTime >= now &&
                                           fsi.FlashSaleEventEntity.IsActive &&
                                           fsi.AvailableStock > 0);
        }

        public async Task<List<FlashSaleItem>> GetFlashSaleItemsByEventAsync(Guid eventId)
        {
            return await _context.FlashSaleItems
                .Include(fsi => fsi.Product)
                .Where(fsi => fsi.FlashSaleEventId == eventId)
                .ToListAsync();
        }

        public async Task AddFlashSaleItemAsync(FlashSaleItem flashSaleItem)
        {
            await _context.FlashSaleItems.AddAsync(flashSaleItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFlashSaleItemAsync(FlashSaleItem flashSaleItem)
        {
            _context.FlashSaleItems.Update(flashSaleItem);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<FlashSaleEventEntity>> GetFlashSalesStartingSoonAsync(TimeSpan timeSpan)
        {
            var now = DateTime.UtcNow;
            var startingSoon = now.Add(timeSpan);

            return await _context.FlashSaleEventEntities
                .Include(fse => fse.FlashSaleItems)
                    .ThenInclude(fsi => fsi.Product)
                .Where(fse => fse.StartTime > now &&
                             fse.StartTime <= startingSoon &&
                             fse.IsActive)
                .ToListAsync();
        }
        public async Task<IEnumerable<FlashSaleItem>> GetActiveFlashSaleItemsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.FlashSaleItems
                .Include(fsi => fsi.Product)
                .Include(fsi => fsi.FlashSaleEventEntity)
                .Where(fsi => fsi.FlashSaleEventEntity.StartTime <= now &&
                             fsi.FlashSaleEventEntity.EndTime >= now &&
                             fsi.FlashSaleEventEntity.IsActive)
                .ToListAsync();
        }
        #endregion
    }
}
