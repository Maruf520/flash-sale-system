using FlashSale.Application.Events;
using FlashSale.Core.Entities;
using FlashSale.Core.Repositories;
using MediatR;

public class PlaceOrderHandler(
    IOrderRepository _orderRepository,
    IDomainEventDispatcher _domainEventDispatcher
) : IRequestHandler<PlaceOrderCommand, PlaceOrderResult>
{
    public async Task<PlaceOrderResult> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Order;

        var order = Order.Create(
            userId: dto.UserId,
            productId: dto.ProductId,
            flashSaleId: dto.FlashSaleId,
            price: dto.Price,
            expireAt: DateTime.UtcNow.AddMinutes(15),
            paymentMethod: dto.PaymentMethod,
            transactionId: dto.TransactionId
        );

        await _orderRepository.AddAsync(order);

        await _domainEventDispatcher.DispatchAndClearEventsAsync(order);

        return new PlaceOrderResult(order.Id);
    }
}
