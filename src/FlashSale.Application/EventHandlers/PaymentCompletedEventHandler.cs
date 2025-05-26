using FlashSale.Core.Enums;
using FlashSale.Core.Events;
using FlashSale.Core.Repositories;
using FlashSale.Core.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlashSale.Application.EventHandlers
{
    public class PaymentCompletedEventHandler(
   IRedisStockService _redisStockService,
   IOrderRepository _orderRepository,
   ILogger<PaymentCompletedEventHandler> _logger
  ) : INotificationHandler<PaymentCompletedEvent>
    {
        public async Task Handle(PaymentCompletedEvent notification, CancellationToken cancellationToken)
        {
            var order = notification.Order;

            try
            {
                _logger.LogInformation(
                    "Processing payment completion for Order {OrderId}, Payment {PaymentId}",
                    order.Id, notification.PaymentId);

                var stockConfirmed = await _redisStockService.ConfirmStockReservationAsync(
                    order.FlashSaleItemId, 1);

                if (stockConfirmed)
                {
                    order.Status = OrderStatus.Paid;
                    order.UpdatedAt = DateTime.UtcNow;
                    await _orderRepository.UpdateAsync(order);

                    _logger.LogInformation(
                        "Stock confirmed and order updated to PAID for Order {OrderId}",
                        order.Id);

                    // 🔥 SEND CONFIRMATION NOTIFICATIONS (Fire and forget)
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            //await _notificationService.SendOrderConfirmationAsync(order);
                            _logger.LogInformation("Confirmation notifications sent for Order {OrderId}", order.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send confirmation notifications for Order {OrderId}", order.Id);
                        }
                    }, cancellationToken);
                }
                else
                {
                    // Critical error - payment succeeded but stock confirmation failed
                    _logger.LogCritical(
                        "CRITICAL: Payment succeeded but stock confirmation failed for Order {OrderId}",
                        order.Id);

                    order.Status = OrderStatus.PaymentConfirmedStockFailed;
                    order.UpdatedAt = DateTime.UtcNow;
                    await _orderRepository.UpdateAsync(order);

                    // Send critical alert
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            //await _notificationService.SendCriticalAlertAsync(
                            //    $"Stock confirmation failed for paid order {order.Id}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send critical alert for Order {OrderId}", order.Id);
                        }
                    }, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error processing payment completion for Order {OrderId}",
                    order.Id);
            }
        }
    }
}
