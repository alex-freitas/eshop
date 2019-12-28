using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.Application.Commands
{

    public class SetAwaitingValidationOrderStatusCommandHandler : IRequestHandler<SetAwaitingValidationOrderStatusCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;

        public SetAwaitingValidationOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<bool> Handle(SetAwaitingValidationOrderStatusCommand request, CancellationToken cancellationToken)
        {
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
