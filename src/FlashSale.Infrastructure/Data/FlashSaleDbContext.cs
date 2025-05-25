namespace FlashSale.Infrastructure.Data
{
    public class FlashSaleDbContext : DbContext
    {
        public FlashSaleDbContext(DbContextOptions<FlashSaleDbContext> options)
        : base(options) { }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<FlashDeal> FlashDeals { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<PaymentInfo> PaymentInfos { get; set; }
        public DbSet<ApplicationRole> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(FlashSaleDbContext).Assembly);
        }
    }
}
