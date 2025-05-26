namespace FlashSale.Core.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = default!;
        public virtual Address? Address { get; set; }
        public ICollection<PaymentInfo> PaymentInfos { get; set; } = new List<PaymentInfo>();
    }
}
