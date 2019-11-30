namespace Ordering.Application.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Dapper;

    public class OrderQueries : IOrderQueries
    {
        private readonly string _connectionString;

        public OrderQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var sql = @"
select o.[Id] as ordernumber,o.OrderDate as date, o.Description as description,
o.Address_City as city, o.Address_Country as country, o.Address_State as state, o.Address_Street as street, o.Address_ZipCode as zipcode,
os.Name as status, 
oi.ProductName as productname, oi.Units as units, oi.UnitPrice as unitprice, oi.PictureUrl as pictureurl
FROM ordering.Orders o
LEFT JOIN ordering.Orderitems oi ON o.Id = oi.orderid 
LEFT JOIN ordering.orderstatus os on o.OrderStatusId = os.Id
";

                var result = await connection.QueryAsync<dynamic>(sql);

                if (result.AsList().Count == 0)
                {
                    throw new KeyNotFoundException();
                }

                return MapOrders(result);
            }
        }

        public async Task<Order> GetOrderAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var sql = @"
select o.[Id] as ordernumber,o.OrderDate as date, o.Description as description,
o.Address_City as city, o.Address_Country as country, o.Address_State as state, o.Address_Street as street, o.Address_ZipCode as zipcode,
os.Name as status, 
oi.ProductName as productname, oi.Units as units, oi.UnitPrice as unitprice, oi.PictureUrl as pictureurl
FROM ordering.Orders o
LEFT JOIN ordering.Orderitems oi ON o.Id = oi.orderid 
LEFT JOIN ordering.orderstatus os on o.OrderStatusId = os.Id
WHERE o.Id=@id
";

                var result = await connection.QueryAsync<dynamic>(sql, new { id });

                if (result.AsList().Count == 0)
                {
                    throw new KeyNotFoundException();
                }

                return MapOrderItems(result);
            }
        }

        private Order MapOrderItems(dynamic result)
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

        private IEnumerable<Order> MapOrders(dynamic result)
        {
            foreach (var item in result)
            {
                yield return MapOrderItems(item);
            }
        }
    }
}
