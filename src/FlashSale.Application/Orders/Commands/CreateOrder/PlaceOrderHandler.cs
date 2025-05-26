using FlashSale.Application.Events;
using FlashSale.Core.Entities;
using FlashSale.Core.Repositories;
using MediatR;

public class PlaceOrderHandler(
    IOrderRepository _orderRepository, IFlashSaleRepository _flashSaleRepository,
    IDomainEventDispatcher _domainEventDispatcher
) : IRequestHandler<PlaceOrderCommand, PlaceOrderResult>
{
    public async Task<PlaceOrderResult> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Order;

        // Get FlashSaleItem by ProductId and FlashSaleEventId (from our earlier discussion)
        var flashSaleItem = await _flashSaleRepository
            .GetFlashSaleItemByProductAndEventAsync(dto.ProductId, dto.FlashSaleEventId);

        if (flashSaleItem == null)
        {
            throw new InvalidOperationException("Product not found in this flash sale event");
        }

        //var now = DateTime.UtcNow;
        //if (flashSaleItem.FlashSaleEventEntity.StartTime > now ||
        //    flashSaleItem.FlashSaleEventEntity.EndTime < now ||
        //    !flashSaleItem.FlashSaleEventEntity.IsActive)
        //{
        //    throw new InvalidOperationException("Flash sale is not active");
        //}

        if (flashSaleItem.AvailableStock <= 0)
        {
            throw new InvalidOperationException("Product is out of stock");
        }

        var order = Order.Create(
            userId: dto.UserId,                           
            productId: dto.ProductId,
            flashSaleItemId: flashSaleItem.Id,           
            price: flashSaleItem.DiscountedPrice,        
            expireAt: DateTime.UtcNow.AddMinutes(15),
            paymentMethod: dto.PaymentMethod,
            transactionId: dto.TransactionId
        );

        await _orderRepository.AddAsync(order);
        await _domainEventDispatcher.DispatchAndClearEventsAsync(order);

        return new PlaceOrderResult(order.Id);
    }
}
