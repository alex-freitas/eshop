using System;
using System.Collections.Generic;
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
        private readonly IntegrationEventLogContext _integrationEventLogContext;
        private readonly List<Type> _eventTypes;

        public IntegrationEventLogService(DbContextOptions<IntegrationEventLogContext> dbContextOptions)
            : this()
        {
            var options = dbContextOptions ?? throw new ArgumentNullException(nameof(dbContextOptions));

            _integrationEventLogContext = new IntegrationEventLogContext(options);
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
            try
            {


                var id = transactionId.ToString();

                var result = await _integrationEventLogContext.IntegrationEventLogs
                    .Where(e => e.TransactionId == id && e.State == EventState.NotPublished)
                    .ToListAsync();

                if (result != null && result.Any())
                {

                    return result
                        .OrderBy(e => e.CreationTime)
                        .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)));
                }

            }
            catch (Exception ex)
            {

                throw;
            }

            return new List<IntegrationEventLogEntry>();
        }

        public Task SaveEventAsync(IntegrationEvent integrationEvent, IDbContextTransaction transaction)
        {
            try
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
            catch (Exception ex)
            {
                throw;
            }

        }

        private Task UpdateEventStatus(Guid eventId, EventState status)
        {
            try
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
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
