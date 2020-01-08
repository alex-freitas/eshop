using System.Collections.Generic;
using MediatR;
using Ordering.Application.Dtos;
using Ordering.Application.Models;

namespace Ordering.Application.Commands
{
    public class CreateOrderDraftCommand : IRequest<OrderDraftDto>
    {
        public CreateOrderDraftCommand(string buyerId, IEnumerable<BasketItem> items)
        {
            BuyerId = buyerId;
            Items = items;
        }

        public string BuyerId { get; private set; }

        public IEnumerable<BasketItem> Items { get; private set; }
    }
}
