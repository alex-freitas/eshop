using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EventBus.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace IntegrationEventLog.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly DbConnection _dbConnection;
        private readonly IntegrationEventLogContext _integrationEventLogContext;
        private readonly List<Type> _eventTypes;

        public IntegrationEventLogService(DbConnection dbConnection)
            : this()
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }

        public IntegrationEventLogService(IntegrationEventLogContext integrationEventLogContext)
            : this()
        {
            _integrationEventLogContext = integrationEventLogContext ?? throw new ArgumentNullException(nameof(integrationEventLogContext));
        }

        private IntegrationEventLogService()
        {
            var assembly = Assembly.Load(Assembly.GetEntryAssembly().FullName);

            _eventTypes = assembly.GetTypes()
                .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventState.PublishedFailed);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventState.InProgress);
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventState.Published);
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
        {
            var id = transactionId.ToString();

            var logs = await _integrationEventLogContext.IntegrationEventLogs
                .Where(e => e.TransactionId == id && e.State == EventState.NotPublished)
                .OrderBy(e => e.CreationTime)
                .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)))
                .ToListAsync();

            return logs;
        }

        public Task SaveEventAsync(IntegrationEvent integrationEvent, IDbContextTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var eventLogEntry = new IntegrationEventLogEntry(integrationEvent, transaction.TransactionId);

            _integrationEventLogContext.Database.UseTransaction(transaction.GetDbTransaction());
            _integrationEventLogContext.IntegrationEventLogs.Add(eventLogEntry);

            return _integrationEventLogContext.SaveChangesAsync();
        }

        private Task UpdateEventStatus(Guid eventId, EventState status)
        {
            var log = _integrationEventLogContext.IntegrationEventLogs.Single(e => e.EventId == eventId);
            log.State = status;

            if (status == EventState.InProgress)
            {
                log.TimesSent++;
            }

            _integrationEventLogContext.IntegrationEventLogs.Update(log);

            return _integrationEventLogContext.SaveChangesAsync();
        }
    }
}
