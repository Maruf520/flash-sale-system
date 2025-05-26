using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashSale.Core.Events
{
    public class OrderExpiredEvent : DomainEvent
    {
        public Order Order { get; }

        public OrderExpiredEvent(Order order)
        {
            Order = order;
        }
    }
}
