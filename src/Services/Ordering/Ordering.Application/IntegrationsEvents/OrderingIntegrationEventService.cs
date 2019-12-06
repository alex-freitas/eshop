using System;
using System.Threading.Tasks;
using EventBus.Abstractions;
using EventBus.Events;
using IntegrationEventLog;
using IntegrationEventLog.Services;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure;

namespace Ordering.Application.IntegrationsEvents
{
    public class OrderingIntegrationEventService : IOrderingIntegrationEventService
    {
        private readonly IEventBus _eventBus;
        private readonly OrderingSqliteContext _orderingContext;
        private readonly IntegrationEventLogContext _eventLogContext;
        private readonly IIntegrationEventLogService _integrationEventLogService;
        private readonly ILogger<OrderingIntegrationEventService> _logger;

        public OrderingIntegrationEventService(
            IEventBus eventBus,
            OrderingSqliteContext orderingContext,
            IntegrationEventLogContext eventLogContext,
            IIntegrationEventLogService integrationEventLogService,
            ILogger<OrderingIntegrationEventService> logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));
            _eventLogContext = eventLogContext ?? throw new ArgumentNullException(nameof(eventLogContext));
            _integrationEventLogService = integrationEventLogService ?? throw new ArgumentNullException(nameof(integrationEventLogService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            _logger.LogInformation("----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);

            var currentTransaction = _orderingContext.GetCurrentTransaction();

            await _integrationEventLogService.SaveEventAsync(evt, currentTransaction);
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pending = await _integrationEventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (var log in pending)
            {
                _logger.LogInformation($"----- Publishing integration event: {log.EventId} from {"AppName"} - ({log.IntegrationEvent})");

                try
                {
                    await _integrationEventLogService.MarkEventAsInProgressAsync(log.EventId);

                    _eventBus.Publish(log.IntegrationEvent);

                    await _integrationEventLogService.MarkEventAsPublishedAsync(log.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"ERROR publishing integration event: {log.EventId} from {"AppName"}");
                    await _integrationEventLogService.MarkEventAsFailedAsync(log.EventId);
                }
            }
        }
    }
}
