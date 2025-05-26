namespace FlashSale.Infrastructure.Data.Configurations
{
    public class DomainEventConfiguration : IEntityTypeConfiguration<DomainEvent>
    {
        public void Configure(EntityTypeBuilder<DomainEvent> builder)
        {
            builder.HasNoKey();
        }
    }
}
