using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IntegrationEventLog
{
    public class IntegrationEventLogContext : DbContext
    {
        public IntegrationEventLogContext(DbContextOptions<IntegrationEventLogContext> options) : base(options)
        {
            if (IsSqlite)
            {
                //Database.EnsureCreated();
            }
        }

        public bool IsSqlite => Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite";

        public DbSet<IntegrationEventLogEntry> IntegrationEventLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IntegrationEventLogEntry>(ConfigureIntegrationEventLogEntry);
        }

        void ConfigureIntegrationEventLogEntry(EntityTypeBuilder<IntegrationEventLogEntry> builder)
        {
            builder.ToTable("IntegrationEventLog");

            builder.HasKey(e => e.EventId);

            builder.Property(e => e.EventId).IsRequired();

            builder.Property(e => e.Content).IsRequired();

            builder.Property(e => e.CreationTime).IsRequired();

            builder.Property(e => e.State).IsRequired();

            builder.Property(e => e.TimesSent).IsRequired();

            builder.Property(e => e.EventTypeName).IsRequired();

            builder.Property(e => e.TransactionId).IsRequired();
        }
    }
}
