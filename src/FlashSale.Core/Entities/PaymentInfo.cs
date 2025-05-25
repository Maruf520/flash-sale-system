namespace FlashSale.Core.Entities
{
    public class PaymentInfo : BaseEntity
    {
        public Guid UserId { get; set; }
        public string CardHolderName { get; set; } = default!;
        public string CardNumber { get; set; } = default!;
        public string Expiry { get; set; } = default!;
        public string CVV { get; set; } = default!;

        public ApplicationUser User { get; set; } = default!;
    }
}
