using System.Collections.Generic;

namespace Ordering.Application.Models
{
    public class CustomerBasket
    {
        public CustomerBasket(string customerId)
        {
            BuyerId = customerId;
            Items = new List<BasketItem>();
        }

        public string BuyerId { get; }

        public List<BasketItem> Items { get; }
    }
}
