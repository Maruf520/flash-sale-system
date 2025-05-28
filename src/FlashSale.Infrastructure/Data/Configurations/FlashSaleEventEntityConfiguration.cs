using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashSale.Infrastructure.Data.Configurations
{
    public class FlashSaleEventEntityConfiguration : IEntityTypeConfiguration<FlashSaleEventEntity>
    {
        public void Configure(EntityTypeBuilder<FlashSaleEventEntity> builder)
        {
            builder.ToTable("FlashSaleEvents");

            builder.HasKey(fse => fse.Id);
            builder.Property(fse => fse.Name).IsRequired().HasMaxLength(200);
            builder.Property(fse => fse.Description).HasMaxLength(1000);
            builder.Property(fse => fse.StartTime).IsRequired();
            builder.Property(fse => fse.EndTime).IsRequired();
            builder.Property(fse => fse.IsActive).IsRequired().HasDefaultValue(false);

            builder.HasMany(fse => fse.FlashSaleItems)
                   .WithOne(fsi => fsi.FlashSaleEventEntity)
                   .HasForeignKey(fsi => fsi.FlashSaleEventId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
