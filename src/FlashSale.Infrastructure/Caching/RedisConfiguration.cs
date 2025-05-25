namespace FlashSale.Infrastructure.Caching
{
    public static class RedisConfiguration
    {
        public static IServiceCollection AddRedisCaching(this IServiceCollection services, IConfiguration configuration)
        {
            var redisOptions = new RedisOptions();
            configuration.GetSection("Redis").Bind(redisOptions);

            services.AddSingleton(redisOptions);

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisOptions.Configuration));

            services.AddSingleton<IRedisCacheService, RedisCacheService>();

            return services;
        }
    }
}
