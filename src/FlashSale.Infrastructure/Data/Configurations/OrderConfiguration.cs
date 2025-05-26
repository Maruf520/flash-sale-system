namespace FlashSale.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(o => o.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(o => o.ExpireAt)
                .IsRequired();

           
            builder.Property(o => o.PaymentId)
                .HasMaxLength(100);

            builder.Property(o => o.PaymentMethod)
                .HasMaxLength(50);

            builder.Property(o => o.TransactionId)
                .HasMaxLength(100);

            builder.Property(o => o.CancellationReason)
                .HasMaxLength(500);

        
            builder.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Product)
                .WithMany()
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.FlashSaleItem)
                .WithMany()
                .HasForeignKey(o => o.FlashSaleItemId)
                .OnDelete(DeleteBehavior.Restrict);

        
            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => o.Status);
            builder.HasIndex(o => o.ExpireAt);
            builder.HasIndex(o => o.PaymentId);
        }
    }
}
