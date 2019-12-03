using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Ordering.Domain.SharedKernel;

namespace Ordering.Infrastructure.Extensions
{
    internal static class MediatorExtensions
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, OrderingContext context)
        {
            var domainEntities = context.ChangeTracker
                .Entries<Entity>()
                .Where(e => e.Entity.DomainEvents != null && e.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList().ForEach(e => e.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}
