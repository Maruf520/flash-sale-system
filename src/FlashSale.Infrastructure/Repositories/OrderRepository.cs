using FlashSale.Core.Enums;
using Order = FlashSale.Core.Entities.Order;

namespace FlashSale.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {

        private readonly FlashSaleDbContext _context;

        public OrderRepository(FlashSaleDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Product)
                .Include(o => o.FlashSaleItem)                    
                    .ThenInclude(fsi => fsi.FlashSaleEventEntity) 
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Orders
                .Include(o => o.Product)
                .Include(o => o.FlashSaleItem)                    
                    .ThenInclude(fsi => fsi.FlashSaleEventEntity) 
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)              
                .ToListAsync();
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetByFlashSaleEventIdAsync(Guid flashSaleEventId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Product)
                .Include(o => o.FlashSaleItem)
                    .ThenInclude(fsi => fsi.FlashSaleEventEntity)
                .Where(o => o.FlashSaleItem.FlashSaleEventId == flashSaleEventId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Product)
                .Include(o => o.FlashSaleItem)
                    .ThenInclude(fsi => fsi.FlashSaleEventEntity)
                .Where(o => o.Status == OrderStatus.Pending)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetExpiredOrdersAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Orders
                .Include(o => o.FlashSaleItem)
                .Where(o => o.Status == OrderStatus.Pending && o.ExpireAt < now)
                .ToListAsync();
        }
    }
}

