using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.SharedKernel;

namespace Ordering.Infrastructure.Repositories
{
    public class BuyerSqliteRepository : IBuyerRepository
    {
        private readonly OrderingSqliteDbContext _context;

        public BuyerSqliteRepository(OrderingSqliteDbContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public Buyer Add(Buyer buyer)
        {
            return buyer.IsTransient() ? _context.Buyers.Add(buyer).Entity : buyer;
        }

        public async Task<Buyer> FindAsync(string buyerIdentityGuid) => await _context.Buyers
                .Include(b => b.PaymentMethods)
                .Where(b => b.IdentityGuid == buyerIdentityGuid)
                .SingleOrDefaultAsync();

        public async Task<Buyer> FindByIdAsync(string id) => await _context.Buyers
                .Include(b => b.PaymentMethods)
                .Where(b => b.Id == int.Parse(id))
                .SingleOrDefaultAsync();

        public Buyer Update(Buyer buyer) => _context.Buyers.Update(buyer).Entity;
    }
}
