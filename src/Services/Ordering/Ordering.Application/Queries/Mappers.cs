namespace Ordering.Application.Queries
{
    using System.Collections.Generic;

    public static class Mappers
    {
        public static Order MapOrderItems(dynamic result)
        {
            var order = new Order
            {
                ordernumber = result[0].ordernumber,
                date = result[0].date,
                status = result[0].status,
                description = result[0].description,
                street = result[0].street,
                city = result[0].city,
                zipcode = result[0].zipcode,
                country = result[0].country,
                orderitems = new List<OrderItem>(),
                total = 0,
            };

            foreach (dynamic item in result)
            {
                var orderitem = new OrderItem
                {
                    productname = item.productname,
                    units = item.units,
                    unitprice = (double)item.unitprice,
                    pictureurl = item.pictureurl
                };

                order.total += item.units * item.unitprice;
                order.orderitems.Add(orderitem);
            }

            return order;
        }

        public static IEnumerable<Order> MapOrders(dynamic result)
        {
            foreach (var item in result)
            {
                yield return MapOrderItems(item);
            }
        }
    }
}
