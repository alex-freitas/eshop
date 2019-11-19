namespace Ordering.Domain.Events
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using Ordering.Domain.AggregatesModel.OrderAggregate;

    public class OrderStartedDomainEvent : INotification
    {
        public OrderStartedDomainEvent(
            Order order,
            string userId,
            string userName,
            int cardTypeId,
            string cardNumber,
            string cardSecurityNumber,
            string cardHolderName,
            DateTime cardExpiration)
        {
            Order = order;
            UserId = userId;
            UserName = userName;
            CardTypeId = cardTypeId;
            CardNumber = cardNumber;
            CardSecurityNumber = cardSecurityNumber;
            CardHolderName = cardHolderName;
            CardExpiration = cardExpiration;
        }

        public string UserId { get; }

        public string UserName { get; }

        public int CardTypeId { get; }

        public string CardNumber { get; }

        public string CardSecurityNumber { get; }

        public string CardHolderName { get; }

        public DateTime CardExpiration { get; }

        public Order Order { get; }
    }

    public class OrderStatusChangedToAwaitingValidationDomainEvent : INotification
    {
        public OrderStatusChangedToAwaitingValidationDomainEvent(int orderId, IEnumerable<OrderItem> orderItems)
        {
            OrderId = orderId;
            OrderItems = orderItems;
        }

        public int OrderId { get; }

        public IEnumerable<OrderItem> OrderItems { get; }
    }

    public class OrderStatusChangedToStockConfirmedDomainEvent : INotification
    {
        public OrderStatusChangedToStockConfirmedDomainEvent(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; }
    }

    public class OrderStatusChangedToPaidDomainEvent : INotification
    {
        public OrderStatusChangedToPaidDomainEvent(int orderId, IEnumerable<OrderItem> orderItems)
        {
            OrderId = orderId;
            OrderItems = orderItems;
        }

        public int OrderId { get; }

        public IEnumerable<OrderItem> OrderItems { get; }
    }

    public class OrderShippedDomainEvent : INotification
    {
        public OrderShippedDomainEvent(Order order)
        {
            Order = order;
        }

        public Order Order { get; }
    }

    public class OrderCancelledDomainEvent : INotification
    {
        public OrderCancelledDomainEvent(Order order)
        {
            Order = order;
        }

        public Order Order { get; }
    }
}
