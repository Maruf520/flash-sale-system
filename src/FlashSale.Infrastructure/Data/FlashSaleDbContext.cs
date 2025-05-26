using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlashSale.Infrastructure.Data
{
    public class FlashSaleDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public FlashSaleDbContext(DbContextOptions<FlashSaleDbContext> options)
        : base(options) { }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Core.Entities.Order> Orders { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<PaymentInfo> PaymentInfos { get; set; }
        public DbSet<ApplicationRole> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<FlashSaleEventEntity> FlashSaleEventEntities { get; set; }
        public DbSet<FlashSaleItem> FlashSaleItems { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(FlashSaleDbContext).Assembly);
            builder.Ignore<DomainEvent>();
        }
    }
}
