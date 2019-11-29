using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.SharedKernel;

namespace Ordering.Infrasctructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderingContext _context;

        public OrderRepository(OrderingContext context)
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

        public void Update(Order order) => _context.Entry(order).State = EntityState.Modified;
    }
}
