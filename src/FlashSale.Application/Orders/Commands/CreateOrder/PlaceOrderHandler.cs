using FlashSale.Core.Entities;
using FlashSale.Core.Repositories;
using FlashSale.Core.Services;
using MediatR;
using Microsoft.Extensions.Logging;

public class PlaceOrderHandler(
    IOrderRepository _orderRepository, IFlashSaleRepository _flashSaleRepository, IRedisStockService _redisStockService, ILogger<PlaceOrderHandler> _logger,
IDomainEventDispatcher _domainEventDispatcher
) : IRequestHandler<PlaceOrderCommand, PlaceOrderResult>
{
    public async Task<PlaceOrderResult> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Order;

        var flashSaleItem = await _flashSaleRepository
            .GetFlashSaleItemByProductAndEventAsync(dto.ProductId, dto.FlashSaleEventId);

        if (flashSaleItem == null)
        {
            throw new InvalidOperationException("Product not found in this flash sale event");
        }


        var now = DateTime.UtcNow;
        if (flashSaleItem.FlashSaleEventEntity.StartTime > now ||
            flashSaleItem.FlashSaleEventEntity.EndTime < now ||
            !flashSaleItem.FlashSaleEventEntity.IsActive)
        {
            throw new InvalidOperationException("Flash sale is not active");
        }


        var stockReserved = await _redisStockService.ReserveStockAsync(
            flashSaleItem.Id,
            quantity: 1,
            reservationTtlMinutes: 15);

        if (!stockReserved)
        {
            _logger.LogWarning(
                "Failed to reserve stock for FlashSaleItem {FlashSaleItemId} for User {UserId}",
                flashSaleItem.Id, dto.UserId);
            throw new InvalidOperationException("Product is out of stock");
        }

        try
        {
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

            _logger.LogInformation(
                "Order {OrderId} created successfully for FlashSaleItem {FlashSaleItemId}",
                order.Id, flashSaleItem.Id);

            return new PlaceOrderResult(order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to create order for FlashSaleItem {FlashSaleItemId}, releasing reserved stock",
                flashSaleItem.Id);

            await _redisStockService.ReleaseStockAsync(flashSaleItem.Id, 1);
            throw;
        }
    }
}
