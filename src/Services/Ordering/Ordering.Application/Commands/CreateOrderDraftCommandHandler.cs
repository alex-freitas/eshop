using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ordering.Application.Dtos;
using Ordering.Application.Extensions;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.Application.Commands
{
    public class CreateOrderDraftCommandHandler : IRequestHandler<CreateOrderDraftCommand, OrderDraftDto>
    {
        public Task<OrderDraftDto> Handle(CreateOrderDraftCommand request, CancellationToken cancellationToken)
        {
            var order = Order.NewDraft();

            var orderItems = request.Items.Select(i => i.ToOrderItemDto());

            foreach (var item in orderItems)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, item.PictureUrl, item.Units);
            }

            return Task.FromResult(OrderDraftDto.FromOrder(order));
        }
    }
}
