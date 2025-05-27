public class PlaceOrderHandler(
    IOrderRepository _orderRepository, IFlashSaleRepository _flashSaleRepository, IRedisStockService _redisStockService, ILogger<PlaceOrderHandler> _logger,
IDomainEventDispatcher _domainEventDispatcher
) : IRequestHandler<PlaceOrderCommand, PlaceOrderResult>
{
    public async Task<PlaceOrderResult> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Order;

        var flashSalevent = await _flashSaleRepository
            .GetActiveFlashSaleEventAsync(dto.FlashSaleEventId);

        if (flashSalevent == null)
        {
            throw new InvalidOperationException("Product not found in this flash sale event");
        }

        var now = DateTime.UtcNow;
        if (flashSalevent.StartTime > now ||
            flashSalevent.EndTime < now ||
            !flashSalevent.IsActive)
        {
            throw new InvalidOperationException("Flash sale is not active");
        }

        var availableStock = await _redisStockService.GetAvailableStockAsync(dto.FlashSaleItemId);
        _logger.LogInformation("Available stock for FlashSaleItem {FlashSaleItemId}: {Stock}",
            dto.FlashSaleItemId, availableStock);

        if (availableStock < 1)
        {
            throw new InvalidOperationException("Product is out of stock");
        }

        var stockReserved = await _redisStockService.ReserveStockAsync(
            dto.FlashSaleItemId,
            quantity: 1,
            reservationTtlMinutes: 15);

        if (!stockReserved)
        {
            _logger.LogWarning("Failed to reserve stock for FlashSaleItem {FlashSaleItemId} for User {UserId}",
                dto.FlashSaleItemId, dto.UserId);
            throw new InvalidOperationException("Product is out of stock");
        }

        var productItem = await _flashSaleRepository.GetFlashSaleItemByIdAsync(request.Order.FlashSaleItemId);

        try
        {
            var order = Order.Create(
                userId: new Guid("7B99CFC9-1B09-47F0-B45B-133AA1652999"),
                productId: productItem.ProductId,
                flashSaleItemId: dto.FlashSaleItemId,
                price: productItem.DiscountedPrice,
                expireAt: DateTime.UtcNow.AddMinutes(15),
                paymentMethod: dto.PaymentMethod,
                transactionId: dto.TransactionId
            );

            await _orderRepository.AddAsync(order);
            await _domainEventDispatcher.DispatchAndClearEventsAsync(order);

            _logger.LogInformation(
                "Order {OrderId} created successfully for FlashSaleItem {FlashSaleItemId}",
                order.Id, flashSalevent.Id);

            return new PlaceOrderResult(order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to create order for FlashSaleItem {FlashSaleItemId}, releasing reserved stock",
                flashSalevent.Id);

            await _redisStockService.ReleaseStockAsync(dto.FlashSaleItemId, 1);
            throw;
        }
    }
}
