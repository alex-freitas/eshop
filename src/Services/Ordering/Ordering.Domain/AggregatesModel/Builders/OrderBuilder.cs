using System;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.Domain.AggregatesModel.Builders
{
    public class OrderBuilder
    {
        private readonly Order _order;

        public OrderBuilder(Address address)
        {
            _order = new Order(
                "userId",
                "fakeName",
                address,
                cardTypeId: 5,
                cardNumber: "12",
                cardSecurityNumber: "123",
                cardHolderName: "name",
                cardExpiration: DateTime.UtcNow);
        }

        public OrderBuilder AddOne(
            int productId,
            string productName,
            decimal unitPrice,
            decimal discount,
            string pictureUrl,
            int units = 1)
        {
            _order.AddOrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
            return this;
        }

        public Order Build()
        {
            return _order;
        }
    }
}
