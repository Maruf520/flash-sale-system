namespace FlashSale.Infrastructure.Caching
{
    public class RedisOptions
    {
        public string Configuration { get; set; } = default!;
        public string InstanceName { get; set; } = "FlashSale:";
    }
}
