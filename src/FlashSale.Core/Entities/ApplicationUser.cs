namespace FlashSale.Core.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = default!;
        public virtual Address Address { get; set; } = default!;
        public virtual PaymentInfo PaymentInfo { get; set; } = default!;
    }
}
