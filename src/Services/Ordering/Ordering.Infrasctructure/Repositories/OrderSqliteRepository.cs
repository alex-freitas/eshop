using System;
using System.Threading.Tasks;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.SharedKernel;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderSqliteRepository : IOrderRepository
    {
        private readonly OrderingSqliteDbContext _context;

        public OrderSqliteRepository(OrderingSqliteDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Order> GetAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order != null)
            {
                await _context.Entry(order).Collection(e => e.OrderItems).LoadAsync();
                await _context.Entry(order).Reference(e => e.OrderStatus).LoadAsync();
                await _context.Entry(order).Reference(e => e.Address).LoadAsync();
            }

            return order;
        }

        public Order Add(Order order) => _context.Orders.Add(order).Entity;

        public void Update(Order order)
        {
            // duvida
        }
    }
}
