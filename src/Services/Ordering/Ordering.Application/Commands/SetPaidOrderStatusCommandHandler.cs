using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.Application.Commands
{
    public class SetPaidOrderStatusCommandHandler : IRequestHandler<SetPaidOrderStatusCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;

        public SetPaidOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<bool> Handle(SetPaidOrderStatusCommand request, CancellationToken cancellationToken)
        {
            await Task.Delay(10000, cancellationToken); // Simulate validating payment

            var order = await _orderRepository.GetAsync(request.OrderNumber);

            if (order == null)
            {
                return false;
            }

            order.SetPaidStatus();

            return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
