namespace Ordering.Domain.Events
{
    using MediatR;

    public class OrderStatusChangedToStockConfirmedDomainEvent : INotification
    {
        public OrderStatusChangedToStockConfirmedDomainEvent(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; }
    }
}
