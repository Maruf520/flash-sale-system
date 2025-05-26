using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashSale.Core.Events
{
    public class PaymentFailedEvent : DomainEvent
    {
        public Order Order { get; }
        public string PaymentId { get; }
        public string FailureReason { get; }

        public PaymentFailedEvent(Order order, string paymentId, string failureReason)
        {
            Order = order;
            PaymentId = paymentId;
            FailureReason = failureReason;
        }
    }
}
