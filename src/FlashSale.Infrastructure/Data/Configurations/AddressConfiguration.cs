namespace FlashSale.Infrastructure.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Street).HasMaxLength(200);
            builder.Property(a => a.City).HasMaxLength(100);
            builder.Property(a => a.Country).HasMaxLength(100);

            builder
                .HasOne(a => a.User)
                .WithOne(u => u.Address)
                .HasForeignKey<Address>(a => a.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
