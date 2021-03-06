﻿namespace Ordering.Domain.Events
{
    using System.Collections.Generic;
    using MediatR;
    using Ordering.Domain.AggregatesModel.OrderAggregate;

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
}
