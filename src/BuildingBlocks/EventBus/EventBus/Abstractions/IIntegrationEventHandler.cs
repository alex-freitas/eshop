using System.Threading.Tasks;
using EventBus.Events;

namespace EventBus.Abstractions
{
    /*    
    http://geekswithblogs.net/abhijeetp/archive/2010/01/10/covariance-and-contravariance-in-c-4.0.aspx 
    */
    public interface IIntegrationEventHandler<in T> : IIntegrationEventHandler
        where T : IntegrationEvent
    {
        Task Handle(T integrationEvent);
    }

    public interface IIntegrationEventHandler
    {
    }
}
