namespace FlashSale.Infrastructure.Services.Redis
{
    public class RedisStockService : IRedisStockService
    {
        private readonly StackExchange.Redis.IDatabase _database;
        private readonly ILogger<RedisStockService> _logger;

        private const string RESERVE_STOCK_SCRIPT = @"
            local stock_key = KEYS[1]
            local reserved_key = KEYS[2] 
            local quantity = tonumber(ARGV[1])
            local ttl = tonumber(ARGV[2])
            
            -- Get current available stock
            local available = tonumber(redis.call('GET', stock_key) or 0)
            
            if available >= quantity then
                -- Decrease available stock
                redis.call('DECRBY', stock_key, quantity)
                -- Increase reserved stock with TTL
                redis.call('INCRBY', reserved_key, quantity)
                redis.call('EXPIRE', reserved_key, ttl)
                return 1
            else
                return 0
            end
        ";

        private const string CONFIRM_RESERVATION_SCRIPT = @"
            local stock_key = KEYS[1]
            local reserved_key = KEYS[2]
            local sold_key = KEYS[3]
            local quantity = tonumber(ARGV[1])
            
            -- Get current reserved stock
            local reserved = tonumber(redis.call('GET', reserved_key) or 0)
            
            if reserved >= quantity then
                -- Move from reserved to sold
                redis.call('DECRBY', reserved_key, quantity)
                redis.call('INCRBY', sold_key, quantity)
                return 1
            else
                return 0
            end
        ";

        private const string RELEASE_STOCK_SCRIPT = @"
    local key = KEYS[1]
    local quantity = tonumber(ARGV[1])
    
    -- Check if hash exists
    if redis.call('EXISTS', key) == 0 then
        return -1  -- Stock not initialized
    end
    
    -- Get current reserved stock from hash
    local reserved = tonumber(redis.call('HGET', key, 'reserved') or 0)
    local available = tonumber(redis.call('HGET', key, 'available') or 0)
    
    if reserved >= quantity then
        -- Return stock from reserved to available (using hash operations)
        redis.call('HSET', key, 'available', available + quantity)
        redis.call('HSET', key, 'reserved', reserved - quantity)
        return 1  -- Success
    else
        return 0  -- Insufficient reserved stock
    end
";

        private const string GET_STOCK_INFO_SCRIPT = @"
        local key = KEYS[1]
        
        -- Check if hash exists
        if redis.call('EXISTS', key) == 0 then
            return {0, 0, 0}  -- available, reserved, sold
        end
        
        local available = tonumber(redis.call('HGET', key, 'available') or 0)
        local reserved = tonumber(redis.call('HGET', key, 'reserved') or 0)
        local sold = tonumber(redis.call('HGET', key, 'sold') or 0)
        
        return {available, reserved, sold}
    ";

        public RedisStockService(IConnectionMultiplexer redis, ILogger<RedisStockService> logger)
        {
            _database = redis.GetDatabase();
            _logger = logger;
        }

        public async Task<int> GetReservedStockAsync(Guid flashSaleItemId)
        {
            try
            {
                var key = $"stock:{flashSaleItemId}";
                var reserved = await _database.HashGetAsync(key, "reserved");

                if (reserved.HasValue)
                {
                    return (int)reserved;
                }

                var keys = GetRedisKeys(flashSaleItemId);
                var reservedSeparate = await _database.StringGetAsync(keys.ReservedKey);

                return reservedSeparate.HasValue ? (int)reservedSeparate : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get reserved stock for {FlashSaleItemId}", flashSaleItemId);
                return 0;
            }
        }

        public async Task<int> GetSoldStockAsync(Guid flashSaleItemId)
        {
            try
            {
                var key = $"stock:{flashSaleItemId}";
                var sold = await _database.HashGetAsync(key, "sold");

                if (sold.HasValue)
                {
                    return (int)sold;
                }

                var keys = GetRedisKeys(flashSaleItemId);
                var soldSeparate = await _database.StringGetAsync(keys.SoldKey);

                return soldSeparate.HasValue ? (int)soldSeparate : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get sold stock for {FlashSaleItemId}", flashSaleItemId);
                return 0;
            }
        }


        public async Task<int> GetTotalStockAsync(Guid flashSaleItemId)
        {
            try
            {
                var key = $"stock:{flashSaleItemId}";
                var total = await _database.HashGetAsync(key, "total");

                return total.HasValue ? (int)total : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get total stock for {FlashSaleItemId}", flashSaleItemId);
                return 0;
            }
        }

        public async Task<bool> ConfirmStockReservationAsync(Guid flashSaleItemId, int quantity = 1)
        {
            var keys = GetRedisKeys(flashSaleItemId);

            try
            {
                var result = await _database.ScriptEvaluateAsync(
                    CONFIRM_RESERVATION_SCRIPT,
                    new RedisKey[] { keys.StockKey, keys.ReservedKey, keys.SoldKey },
                    new RedisValue[] { quantity }
                );

                var success = (int)result == 1;

                _logger.LogInformation(
                    "Stock confirmation for FlashSaleItem {FlashSaleItemId}: {Quantity} units - {Result}",
                    flashSaleItemId, quantity, success ? "SUCCESS" : "FAILED");

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error confirming stock for FlashSaleItem {FlashSaleItemId}: {Quantity} units",
                    flashSaleItemId, quantity);
                return false;
            }
        }

        public async Task<bool> ReleaseStockAsync(Guid flashSaleItemId, int quantity = 1)
        {
            var key = $"stock:{flashSaleItemId}";

            try
            {
                var result = await _database.ScriptEvaluateAsync(
                    RELEASE_STOCK_SCRIPT,
                    new RedisKey[] { key },  
                    new RedisValue[] { quantity }
                );

                var resultCode = (int)result;

                switch (resultCode)
                {
                    case 1:
                        _logger.LogInformation(
                            "Successfully released {Quantity} units for FlashSaleItem {FlashSaleItemId}",
                            quantity, flashSaleItemId);
                        return true;

                    case 0:
                        _logger.LogWarning(
                            "Failed to release {Quantity} units for FlashSaleItem {FlashSaleItemId} - insufficient reserved stock",
                            quantity, flashSaleItemId);
                        return false;

                    case -1:
                        _logger.LogWarning(
                            "Stock not initialized for FlashSaleItem {FlashSaleItemId}",
                            flashSaleItemId);
                        return false;

                    default:
                        _logger.LogWarning(
                            "Unexpected result {Result} from release script for FlashSaleItem {FlashSaleItemId}",
                            resultCode, flashSaleItemId);
                        return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error releasing stock for FlashSaleItem {FlashSaleItemId}: {Quantity} units",
                    flashSaleItemId, quantity);
                return false;
            }
        }

        public async Task<int> GetAvailableStockAsync(Guid flashSaleItemId)
        {
            var key = $"stock:{flashSaleItemId}";

            try
            {
                var result = await _database.ScriptEvaluateAsync(
                    GET_STOCK_INFO_SCRIPT,
                    new RedisKey[] { key }
                );

                var stockInfo = (RedisValue[])result;
                return (int)stockInfo[0]; // available stock
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock info for FlashSaleItem {FlashSaleItemId}", flashSaleItemId);
                return 0;
            }
        }

        public async Task InitializeStockAsync(Guid flashSaleItemId, int initialStock)
        {
            var keys = GetRedisKeys(flashSaleItemId);

            try
            {
                await _database.StringSetAsync(keys.StockKey, initialStock);
                await _database.StringSetAsync(keys.ReservedKey, 0);
                await _database.StringSetAsync(keys.SoldKey, 0);

                _logger.LogInformation(
                    "Initialized stock for FlashSaleItem {FlashSaleItemId}: {InitialStock} units",
                    flashSaleItemId, initialStock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error initializing stock for FlashSaleItem {FlashSaleItemId}: {InitialStock} units",
                    flashSaleItemId, initialStock);
                throw;
            }
        }

        public async Task<bool> IsStockAvailableAsync(Guid flashSaleItemId, int quantity = 1)
        {
            var availableStock = await GetAvailableStockAsync(flashSaleItemId);
            return availableStock >= quantity;
        }

        private RedisKeys GetRedisKeys(Guid flashSaleItemId)
        {
            var prefix = $"flashsale:item:{flashSaleItemId}";
            return new RedisKeys
            {
                StockKey = $"{prefix}:stock",
                ReservedKey = $"{prefix}:reserved",
                SoldKey = $"{prefix}:sold"
            };
        }

        public async Task InitializeStockAsync(Guid flashSaleItemId, int totalQuantity, int soldQuantity, int availableQuantity)
        {
            var key = $"stock:{flashSaleItemId}";

            try
            {
                await _database.HashSetAsync(key, new HashEntry[]
                {
                    new("total", totalQuantity),
                    new("sold", soldQuantity),
                    new("available", availableQuantity),
                    new("reserved", 0),
                    new("synced_at", DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                });

                await _database.KeyExpireAsync(key, TimeSpan.FromHours(24));

                _logger.LogDebug(
                    "Initialized Redis stock for {FlashSaleItemId}: Total={Total}, Sold={Sold}, Available={Available}",
                    flashSaleItemId, totalQuantity, soldQuantity, availableQuantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Redis stock for {FlashSaleItemId}", flashSaleItemId);
                throw;
            }
        }

        public async Task RemoveStockAsync(Guid flashSaleItemId)
        {
            var key = $"stock:{flashSaleItemId}";
            await _database.KeyDeleteAsync(key);
        }

        public async Task<bool> ExistsAsync(Guid flashSaleItemId)
        {
            var key = $"stock:{flashSaleItemId}";
            return await _database.KeyExistsAsync(key);
        }

        public async Task<bool> ReserveStockAsync(Guid flashSaleItemId, int quantity, int reservationTtlMinutes)
        {
            var key = $"stock:{flashSaleItemId}";

            if (!await ExistsAsync(flashSaleItemId))
            {
                _logger.LogWarning("FlashSaleItem {FlashSaleItemId} not synced to Redis", flashSaleItemId);
                return false;
            }

            const string script = @"
            local key = KEYS[1]
            local quantity = tonumber(ARGV[1])
            local ttl = tonumber(ARGV[2])
            
            -- Get current available stock
            local available = tonumber(redis.call('HGET', key, 'available') or 0)
            
            -- Check if we have enough stock
            if available >= quantity then
                -- Atomically update both fields
                redis.call('HSET', key, 'available', available - quantity)
                redis.call('HSET', key, 'reserved', (redis.call('HGET', key, 'reserved') or 0) + quantity)
                redis.call('EXPIRE', key, ttl * 60)
                return 1  -- Success
            else
                return 0  -- Insufficient stock
            end";

            var result = await _database.ScriptEvaluateAsync(
                script,
                new RedisKey[] { key },
                new RedisValue[] { quantity, reservationTtlMinutes });

            var success = (int)result == 1;

            if (success)
            {
                _logger.LogInformation("Reserved {Quantity} units for FlashSaleItem {FlashSaleItemId}",
                    quantity, flashSaleItemId);
            }
            else
            {
                _logger.LogWarning("Failed to reserve {Quantity} units for FlashSaleItem {FlashSaleItemId}",
                    quantity, flashSaleItemId);
            }

            return success;
        }
        private class RedisKeys
        {
            public string StockKey { get; set; } = default!;
            public string ReservedKey { get; set; } = default!;
            public string SoldKey { get; set; } = default!;
        }
    }
}
