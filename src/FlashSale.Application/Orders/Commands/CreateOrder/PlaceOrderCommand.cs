using FlashSale.Application.Dtos;
using FluentValidation;
using MediatR;

public record PlaceOrderCommand(OrderDto Order) : IRequest<PlaceOrderResult>;
public record PlaceOrderResult(Guid Id);

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.Order.UserId).NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.Order.ProductId).NotEmpty().WithMessage("ProductId is required.");
        RuleFor(x => x.Order.FlashSaleId).NotEmpty().WithMessage("FlashSaleId is required.");
        RuleFor(x => x.Order.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}
