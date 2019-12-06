using EventBus.Events;

namespace EventBus.Abstractions
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent integrationEvent);

        void Subscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>;

        void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>;

        void SubscribeDynamic<T>(string eventName)
            where T : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<T>(string eventName)
            where T : IDynamicIntegrationEventHandler;       
    }    
}
