namespace Ordering.Domain.Events
{
    using MediatR;
    using Ordering.Domain.AggregatesModel.OrderAggregate;

    public class OrderShippedDomainEvent : INotification
    {
        public OrderShippedDomainEvent(Order order)
        {
            Order = order;
        }

        public Order Order { get; }
    }
}
