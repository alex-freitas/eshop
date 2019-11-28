using System.Collections.Generic;
using Ordering.Application.Dtos;
using Ordering.Application.Models;

namespace Ordering.Application.Extensions
{
    public static class BasketItemExtensions
    {
        public static IEnumerable<OrderItemDto> ToOrderItemsDto(this IEnumerable<BasketItem> basketItems)
        {
            foreach (var item in basketItems)
            {
                yield return item.ToOrderItemDto();
            }
        }

        public static OrderItemDto ToOrderItemDto(this BasketItem basketItem)
        {
            return new OrderItemDto
            {
                ProductId = int.TryParse(basketItem.ProductId, out int id) ? id : -1,
                ProductName = basketItem.ProductName,
                PictureUrl = basketItem.PictureUrl,
                UnitPrice = basketItem.UnitPrice,
                Units = basketItem.Quantity,
            };
        }
    }
}
