
using Ordering.Domain.SharedKernel;
using System;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    public class Order : Entity
    {
        private readonly DateTime _orderDate;

        public Order()
        {
            _orderDate = DateTime.UtcNow;
        }
    }
}
