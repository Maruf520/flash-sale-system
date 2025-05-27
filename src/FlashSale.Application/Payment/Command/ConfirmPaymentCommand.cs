namespace FlashSale.Application.Payment.Command
{
    public record ConfirmPaymentCommand(PaymentDto Payment) : IRequest<ConfirmPaymentResult>;

    public record ConfirmPaymentResult(bool Success, string Message, Guid OrderId);

    public class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
    {
        public ConfirmPaymentCommandValidator()
        {
            RuleFor(x => x.Payment.OrderId).NotEmpty().WithMessage("OrderId is required.");
            RuleFor(x => x.Payment.PaymentId).NotEmpty().WithMessage("PaymentId is required.");
            RuleFor(x => x.Payment.PaymentMethod).NotEmpty().WithMessage("PaymentMethod is required.");
            RuleFor(x => x.Payment.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
            RuleFor(x => x.Payment.PaymentGatewayResponse).NotEmpty().WithMessage("PaymentGatewayResponse is required.");
        }
    }
}
