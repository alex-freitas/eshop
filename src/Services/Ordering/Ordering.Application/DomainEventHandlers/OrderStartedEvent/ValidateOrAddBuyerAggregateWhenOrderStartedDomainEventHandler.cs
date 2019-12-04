using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.IntegrationsEvents;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.Events;

namespace Ordering.Application.DomainEventHandlers.OrderStartedEvent
{
    public class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler
        : INotificationHandler<OrderStartedDomainEvent>
    {
        private readonly ILoggerFactory _logger;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler(
            ILoggerFactory logger,
            IBuyerRepository buyerRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));

            _orderingIntegrationEventService = orderingIntegrationEventService ??
                throw new ArgumentNullException(nameof(orderingIntegrationEventService));
        }

        public async Task Handle(OrderStartedDomainEvent notification, CancellationToken cancellationToken)
        {
            var cardTypeId = notification.CardTypeId != 0 ? notification.CardTypeId : 1;

            var buyer = await _buyerRepository.FindAsync(notification.UserId);

            bool buyerOriginallyExisted = buyer != null;

            if (!buyerOriginallyExisted)
            {
                buyer = new Buyer(notification.UserId, notification.UserName);
            }

            buyer.VerifyOrAddPaymentMethod(
                cardTypeId,
                $"Payment Method on {DateTime.UtcNow}",
                notification.CardNumber,
                notification.CardSecurityNumber,
                notification.CardHolderName,
                notification.CardExpiration,
                notification.Order.Id);

            var buyerUpdated = buyerOriginallyExisted ?
               _buyerRepository.Update(buyer) :
               _buyerRepository.Add(buyer);

            await _buyerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            _logger
                .CreateLogger<ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler>()
                .LogTrace(
                    "Buyer {BuyerId} and related payment method were validated or updated for orderId: {OrderId}.",
                    buyerUpdated.Id,
                    notification.Order.Id);
        }
    }
}
