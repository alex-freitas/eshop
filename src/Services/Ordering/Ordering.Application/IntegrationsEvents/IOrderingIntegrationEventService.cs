using System;
using System.Threading.Tasks;
using EventBus.Events;

namespace Ordering.Application.IntegrationsEvents
{
    public interface IOrderingIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);

        Task AddAndSaveEventAsync(IntegrationEvent evt);
    }
}
