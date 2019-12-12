using System;
using System.Data.Common;
using System.Threading.Tasks;
using EventBus.Abstractions;
using EventBus.Events;
using IntegrationEventLog;
using IntegrationEventLog.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure;

namespace Ordering.Application.IntegrationsEvents
{
    public class OrderingIntegrationEventService : IOrderingIntegrationEventService
    {
        private readonly ILogger<OrderingIntegrationEventService> _logger;
        //private readonly IEventBus _eventBus;
        private readonly OrderingContext _orderingContext;
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;

        public OrderingIntegrationEventService(
            ILogger<OrderingIntegrationEventService> logger,
            OrderingContext orderingContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));

            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ??
                throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));

            IntegrationEventLogService = _integrationEventLogServiceFactory(_orderingContext.Database.GetDbConnection());
        }

        public IIntegrationEventLogService IntegrationEventLogService { get; }

        public async Task AddAndSaveEventAsync(IntegrationEvent integrationEvent)
        {
            _logger.LogInformation($"----- Enqueuing integration event {integrationEvent.Id} to repository ({integrationEvent})");

            var currentTransaction = _orderingContext.GetCurrentTransaction();

            await IntegrationEventLogService.SaveEventAsync(integrationEvent, currentTransaction);
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pending = await IntegrationEventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (var log in pending)
            {
                _logger.LogInformation($"----- Publishing integration event: {log.EventId} from {"AppName"} - ({log.IntegrationEvent})");

                try
                {
                    await IntegrationEventLogService.MarkEventAsInProgressAsync(log.EventId);

                    //_eventBus.Publish(log.IntegrationEvent);

                    await IntegrationEventLogService.MarkEventAsPublishedAsync(log.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"ERROR publishing integration event: {log.EventId} from {"AppName"}");
                    await IntegrationEventLogService.MarkEventAsFailedAsync(log.EventId);
                }
            }
        }
    }
}
