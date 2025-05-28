namespace FlashSale.Infrastructure.Data.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(u => u.Id);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.PhoneNumber).HasMaxLength(15);

            builder.HasOne(u => u.Address)
                   .WithOne()
                   .HasForeignKey<Address>(a => a.ApplicationUserId);

            builder.HasMany(u => u.PaymentInfos)
                   .WithOne(p => p.User)
                   .HasForeignKey(p => p.UserId);
        }
    }
}
