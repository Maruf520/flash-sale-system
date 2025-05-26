namespace FlashSale.Infrastructure.Data.Configurations
{
    public class FlashSaleConfiguration : IEntityTypeConfiguration<FlashDeal>
    {
        public void Configure(EntityTypeBuilder<FlashDeal> builder)
        {
            builder.HasKey(f => f.Id);
            builder.Property(f => f.DiscountedPrice).HasColumnType("decimal(18,2)");
            builder.Property(f => f.AvailableStock).IsRequired();

            builder.HasOne(f => f.Product)
                   .WithMany(f => f.FlashDeals)
                   .HasForeignKey(f => f.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
