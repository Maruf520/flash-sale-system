namespace FlashSale.Core.Entities
{
    public class Inventory : BaseEntity
    {
        public Guid ProductId { get; set; }
        public int Stock { get; set; }
        public DateTime SnapshotTime { get; set; }

        public Product Product { get; set; } = default!;
    }
}
