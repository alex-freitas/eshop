using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.SharedKernel;
using Ordering.Infrastructure.EntityConfigurations.Sqlite;
using Ordering.Infrastructure.Extensions;

namespace Ordering.Infrastructure
{
    public class OrderingSqliteDbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        public OrderingSqliteDbContext(DbContextOptions<OrderingSqliteDbContext> options, IMediator mediator) : base(options)
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

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("Filename=ordering.db", options =>
        //    {
        //        options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
        //    });

        //    base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PaymentMethodEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CardTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BuyerEntityTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
