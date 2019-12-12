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
using Ordering.Infrastructure.EntityConfigurations;
using SqliteEntityConfigurations = Ordering.Infrastructure.EntityConfigurations.Sqlite;
using Ordering.Infrastructure.Extensions;
using System.Data;

namespace Ordering.Infrastructure
{
    public class OrderingContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "ordering";

        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;

        public OrderingContext(DbContextOptions<OrderingContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            Debug.WriteLine($"{nameof(OrderingContext)}::ctor ->" + GetHashCode());
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

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) 
                return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            ValidateTransaction(transaction);

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        private void ValidateTransaction(IDbContextTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //if (this.IsSqlite())
            //{
                modelBuilder.ApplyConfiguration(new SqliteEntityConfigurations.OrderEntityTypeConfiguration());
                modelBuilder.ApplyConfiguration(new SqliteEntityConfigurations.OrderItemEntityTypeConfiguration());

                modelBuilder.ApplyConfiguration(new SqliteEntityConfigurations.BuyerEntityTypeConfiguration());
                modelBuilder.ApplyConfiguration(new SqliteEntityConfigurations.PaymentMethodEntityTypeConfiguration());

                modelBuilder.ApplyConfiguration(new SqliteEntityConfigurations.CardTypeEntityTypeConfiguration());
                modelBuilder.ApplyConfiguration(new SqliteEntityConfigurations.OrderStatusEntityTypeConfiguration());
            //}
            //else
            //{
                //modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
                //modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());

                //modelBuilder.ApplyConfiguration(new BuyerEntityTypeConfiguration());
                //modelBuilder.ApplyConfiguration(new PaymentMethodEntityTypeConfiguration());

                //modelBuilder.ApplyConfiguration(new CardTypeEntityTypeConfiguration());
                //modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());
            //}


            base.OnModelCreating(modelBuilder);
        }
    }
}
