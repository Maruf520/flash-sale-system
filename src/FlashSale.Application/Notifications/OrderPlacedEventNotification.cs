using FlashSale.Core.Entities;
using MediatR;

namespace FlashSale.Application.Notifications
{
    public class OrderPlacedEventNotification : INotification
    {
        public Order Order { get; }

        public OrderPlacedEventNotification(Order order)
        {
            Order = order;
        }
    }
}
