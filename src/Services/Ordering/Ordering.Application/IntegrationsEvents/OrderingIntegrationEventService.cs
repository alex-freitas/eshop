using System;
using System.Threading.Tasks;
using EventBus.Events;

namespace Ordering.Application.IntegrationsEvents
{
    public class OrderingIntegrationEventService : IOrderingIntegrationEventService
    {
        public Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            return Task.CompletedTask;
        }

        public Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            return Task.CompletedTask;
        }
    }
}
