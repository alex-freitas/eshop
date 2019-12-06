using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.SharedKernel;
using Ordering.Infrastructure.EntityConfigurations.Sqlite;
using Ordering.Infrastructure.Extensions;

namespace Ordering.Infrastructure
{
    public class OrderingSqliteContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;

        public OrderingSqliteContext(DbContextOptions<OrderingSqliteContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            Database.EnsureCreated();

            Debug.WriteLine($"OrderingSqliteDbContext::ctor ->" + GetHashCode());
        }

        public DbSet<Order> Orders { get; set; }
        
        public DbSet<OrderItem> OrderItems { get; set; }
        
        public DbSet<PaymentMethod> Payments { get; set; }
        
        public DbSet<Buyer> Buyers { get; set; }
        
        public DbSet<CardType> CardTypes { get; set; }
        
        public DbSet<OrderStatus> OrderStatus { get; set; }

        public bool HasActiveTransaction => _currentTransaction != null;

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _ = await base.SaveChangesAsync(cancellationToken);

                await _mediator.DispatchDomainEventsAsync(this);

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
            
            modelBuilder.ApplyConfiguration(new BuyerEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentMethodEntityTypeConfiguration());

            modelBuilder.ApplyConfiguration(new CardTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
