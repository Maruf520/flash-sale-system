namespace FlashSale.Core.Entities
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
