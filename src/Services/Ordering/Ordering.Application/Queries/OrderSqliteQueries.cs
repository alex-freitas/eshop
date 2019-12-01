namespace Ordering.Application.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dapper;
    using Microsoft.Data.Sqlite;

    public class OrderSqliteQueries : IOrderQueries
    {
        private readonly string _connectionString;

        public OrderSqliteQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            using (var connection = new SqliteConnection(_connectionString))
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

                return Mappers.MapOrders(result);
            }
        }

        public async Task<Order> GetOrderAsync(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
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

                return Mappers.MapOrderItems(result);
            }
        }
    }
}
