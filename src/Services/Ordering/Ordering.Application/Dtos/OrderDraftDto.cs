using System.Collections.Generic;
using System.Linq;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.Application.Dtos
{
    public class OrderDraftDto
    {
        public IEnumerable<OrderItemDto> OrderItems { get; set; }

        public decimal Total { get; set; }

        public static OrderDraftDto FromOrder(Order order)
        {
            var orderItems = order.OrderItems.Select(oi => new OrderItemDto
            {
                Discount = oi.Discount,
                ProductId = oi.ProductId,
                UnitPrice = oi.UnitPrice,
                PictureUrl = oi.PictureUrl,
                Units = oi.Units,
                ProductName = oi.ProductName,
            });

            var orderDraftDto = new OrderDraftDto
            {
                OrderItems = orderItems,
                Total = order.GetTotal(),
            };

            return orderDraftDto;
        }
    }
}
