
using System;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    public class Order
    {
        private readonly DateTime _orderDate;

        public Order()
        {
            _orderDate = DateTime.UtcNow;
        }
    }
}
