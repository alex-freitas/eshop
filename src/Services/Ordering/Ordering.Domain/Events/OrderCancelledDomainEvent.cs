namespace Ordering.Domain.Events
{
    using MediatR;
    using Ordering.Domain.AggregatesModel.OrderAggregate;

    public class OrderCancelledDomainEvent : INotification
    {
        public OrderCancelledDomainEvent(Order order)
        {
            Order = order;
        }

        public Order Order { get; }
    }
}
