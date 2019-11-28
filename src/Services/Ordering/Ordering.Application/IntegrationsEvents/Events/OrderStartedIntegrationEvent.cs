using EventBus.Events;

namespace Ordering.Application.IntegrationsEvents.Events
{
    public class OrderStartedIntegrationEvent : IntegrationEvent
    {
        public OrderStartedIntegrationEvent(string userId) => UserId = userId;

        public string UserId { get; }
    }
}
