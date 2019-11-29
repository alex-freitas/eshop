using System.Threading.Tasks;
using Ordering.Domain.SharedKernel;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetAsync(int orderId);

        Order Add(Order order);

        void Update(Order order);
    }
}
