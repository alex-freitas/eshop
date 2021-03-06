﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ordering.Domain.Exceptions;
using Ordering.Domain.SharedKernel;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    public class OrderStatus : Enumeration
    {
        public static readonly OrderStatus Submitted = new OrderStatus(1, nameof(Submitted).ToUpperInvariant());
        public static readonly OrderStatus AwaitingValidation = new OrderStatus(2, nameof(AwaitingValidation).ToUpperInvariant());
        public static readonly OrderStatus StockConfirmed = new OrderStatus(3, nameof(StockConfirmed).ToUpperInvariant());
        public static readonly OrderStatus Paid = new OrderStatus(4, nameof(Paid).ToUpperInvariant());
        public static readonly OrderStatus Shipped = new OrderStatus(5, nameof(Shipped).ToUpperInvariant());
        public static readonly OrderStatus Cancelled = new OrderStatus(6, nameof(Cancelled).ToUpperInvariant());

        public OrderStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<OrderStatus> List()
        {
            return new[] { Submitted, AwaitingValidation, StockConfirmed, Paid, Shipped, Cancelled };
        }

        public static OrderStatus FromName(string name)
        {
            var status = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (status == null)
            {
                throw new OrderingDomainException($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
            }

            return status;
        }

        public static OrderStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new OrderingDomainException($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
