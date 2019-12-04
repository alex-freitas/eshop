using MediatR;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.Domain.Events
{
    public class BuyerAndPaymentMethodVerifiedDomainEvent : INotification
    {
        public BuyerAndPaymentMethodVerifiedDomainEvent(Buyer buyer, PaymentMethod payment, int orderId)
        {
            Buyer = buyer;
            Payment = payment;
            OrderId = orderId;
        }

        public BuyerAndPaymentMethodVerifiedDomainEvent(Buyer buyer, PaymentMethod payment, Order order)
            : this(buyer, payment, 0)
        {
            Order = order;
        }

        public Buyer Buyer { get; private set; }

        public PaymentMethod Payment { get; private set; }

        public int OrderId { get; private set; }

        public Order Order { get; set; }
    }
}
