
namespace Ordering.Application.Queries
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IOrderQueries
    {
        Task<IEnumerable<Order>> GetOrdersAsync();

        Task<Order> GetOrderAsync(int id);
    }
}
