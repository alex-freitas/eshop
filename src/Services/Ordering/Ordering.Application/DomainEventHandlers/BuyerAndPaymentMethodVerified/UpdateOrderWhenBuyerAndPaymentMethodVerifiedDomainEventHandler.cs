using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.Events;

namespace Ordering.Application.DomainEventHandlers.BuyerAndPaymentMethodVerified
{
    public class UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler
        : INotificationHandler<BuyerAndPaymentMethodVerifiedDomainEvent>
    {
        private readonly ILoggerFactory _logger;
        private readonly IOrderRepository _orderRepository;

        public UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler(ILoggerFactory logger, IOrderRepository orderRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task Handle(BuyerAndPaymentMethodVerifiedDomainEvent notification, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetAsync(notification.OrderId).ConfigureAwait(true);
            order.SetBuyerId(notification.Buyer.Id);
            order.SetPaymentId(notification.Payment.Id);

            _logger
                .CreateLogger<UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler>()
                .LogTrace(
                    "Order with Id: {OrderId} has been successfully updated with a payment method {PaymentMethod} ({Id})",
                    notification.OrderId,
                    nameof(notification.Payment),
                    notification.Payment.Id);

            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
