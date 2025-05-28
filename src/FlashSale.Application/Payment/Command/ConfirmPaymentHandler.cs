namespace FlashSale.Application.Payment.Command
{
    public class ConfirmPaymentHandler(
           IOrderRepository _orderRepository,
           IDomainEventDispatcher _domainEventDispatcher,
           ILogger<ConfirmPaymentHandler> _logger
       ) : IRequestHandler<ConfirmPaymentCommand, ConfirmPaymentResult>
    {
        public async Task<ConfirmPaymentResult> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Payment;

            try
            {
                _logger.LogInformation(
                    "Processing payment confirmation for Order {OrderId}, Payment {PaymentId}",
                    dto.OrderId, dto.PaymentId);

                var order = await _orderRepository.GetByIdAsync(dto.OrderId);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", dto.OrderId);
                    return new ConfirmPaymentResult(false, "Order not found", dto.OrderId);
                }

                if (order.Status != OrderStatus.Pending)
                {
                    _logger.LogWarning(
                        "Order {OrderId} is not in pending status. Current status: {Status}",
                        dto.OrderId, order.Status);
                    return new ConfirmPaymentResult(false, $"Order is not in pending status. Current status: {order.Status}", dto.OrderId);
                }

                if (order.ExpireAt < DateTime.UtcNow)
                {
                    _logger.LogWarning("Order {OrderId} has expired at {ExpireAt}", dto.OrderId, order.ExpireAt);
                    return new ConfirmPaymentResult(false, "Order has expired", dto.OrderId);
                }

                if (dto.Amount != order.Price)
                {
                    _logger.LogWarning(
                        "Payment amount {PaymentAmount} does not match order amount {OrderAmount} for Order {OrderId}",
                        dto.Amount, order.Price, dto.OrderId);
                    return new ConfirmPaymentResult(false, "Payment amount does not match order amount", dto.OrderId);
                }



                order.PaymentId = dto.PaymentId;
                order.PaymentMethod = dto.PaymentMethod;
                order.PaymentCompletedAt = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;

                order.AddDomainEvent(new PaymentCompletedEvent(order, dto.PaymentId, dto.Amount));

                await _orderRepository.UpdateAsync(order);
                await _domainEventDispatcher.DispatchAndClearEventsAsync(order);

                _logger.LogInformation(
                    "Payment confirmation completed for Order {OrderId}. Event dispatched.",
                    dto.OrderId);

                return new ConfirmPaymentResult(true, "Payment confirmed successfully", dto.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to confirm payment for Order {OrderId}", dto.OrderId);
                return new ConfirmPaymentResult(false, "An error occurred while processing payment", dto.OrderId);
            }
        }
    }
}
