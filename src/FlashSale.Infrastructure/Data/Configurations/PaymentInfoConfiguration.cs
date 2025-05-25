namespace FlashSale.Infrastructure.Data.Configurations
{
    public class PaymentInfoConfiguration : IEntityTypeConfiguration<PaymentInfo>
    {
        public void Configure(EntityTypeBuilder<PaymentInfo> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.CardNumber).HasMaxLength(20);
            builder.Property(p => p.CardHolderName).HasMaxLength(100);
            builder.Property(p => p.Expiry).HasMaxLength(10);

            builder.HasOne(p => p.User)
                   .WithMany(u => u.PaymentInfos)
                   .HasForeignKey(p => p.UserId);
        }
    }
}
