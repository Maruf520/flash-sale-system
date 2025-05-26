namespace FlashSale.Infrastructure.Data.Configurations
{
    public class FlashSaleItemConfiguration : IEntityTypeConfiguration<FlashSaleItem>
    {
        public void Configure(EntityTypeBuilder<FlashSaleItem> builder)
        {
            builder.ToTable("FlashSaleItems");

            builder.HasKey(fsi => fsi.Id);
            builder.Property(fsi => fsi.OriginalPrice).HasColumnType("decimal(18,2)");
            builder.Property(fsi => fsi.DiscountedPrice).HasColumnType("decimal(18,2)");
            builder.Property(fsi => fsi.DiscountPercentage).HasColumnType("decimal(5,2)");
            builder.Property(fsi => fsi.AvailableStock).IsRequired();

            builder.HasOne(fsi => fsi.FlashSaleEventEntity)
                   .WithMany(fse => fse.FlashSaleItems)
                   .HasForeignKey(fsi => fsi.FlashSaleEventId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fsi => fsi.Product)
                   .WithMany(p => p.FlashSaleItems)
                   .HasForeignKey(fsi => fsi.ProductId)  
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
