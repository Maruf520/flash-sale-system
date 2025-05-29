using FlashSale.Infrastructure.Exceptions;
using FlashSale.Infrastructure.Services.Email;

namespace FlashSale.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FlashSaleDbContext>(options =>
                 options.UseSqlServer(configuration.GetConnectionString("Database")));
            var redisConnectionString = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Redis connection string not found");

            services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
            {
                var configuration = ConfigurationOptions.Parse(redisConnectionString);

                configuration.AbortOnConnectFail = false;
                configuration.ConnectRetry = 3;
                configuration.ConnectTimeout = 5000;
                configuration.SyncTimeout = 5000;

                return ConnectionMultiplexer.Connect(configuration);
            });
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddScoped<IFlashSaleRepository, FlashSaleRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRedisSyncService, RedisSyncService>();
            services.AddScoped<IRedisStockService, RedisStockService>();
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<CustomExceptionHandler>();

            return services;
        }
    }
}
