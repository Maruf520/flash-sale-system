namespace FlashSale.Core.Entities
{
    public class Address : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Street { get; set; } = default!;
        public string City { get; set; } = default!;
        public string PostalCode { get; set; } = default!;
        public string Country { get; set; } = default!;

        public ApplicationUser User { get; set; } = default!;
    }
}
