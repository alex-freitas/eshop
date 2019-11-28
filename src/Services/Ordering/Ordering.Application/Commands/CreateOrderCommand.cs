using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MediatR;
using Ordering.Application.Dtos;
using Ordering.Application.Extensions;
using Ordering.Application.Models;

namespace Ordering.Application.Commands
{
    public class CreateOrderCommand : IRequest<bool>
    {
        [DataMember]
        private readonly List<OrderItemDto> _orderItems;

        public CreateOrderCommand()
        {
            _orderItems = new List<OrderItemDto>();
        }

        public CreateOrderCommand(
            List<BasketItem> basketItems,
            string userId,
            string userName,
            string city,
            string street,
            string state,
            string country,
            string zipcode,
            string cardNumber,
            string cardHolderName,
            DateTime cardExpiration,
            string cardSecurityNumber,
            int cardTypeId)
            : this()
        {
            _orderItems = basketItems.ToOrderItemsDto().ToList();
            UserId = userId;
            UserName = userName;
            City = city;
            Street = street;
            State = state;
            Country = country;
            ZipCode = zipcode;
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            CardExpiration = cardExpiration;
            CardSecurityNumber = cardSecurityNumber;
            CardTypeId = cardTypeId;
            CardExpiration = cardExpiration;
        }

        [DataMember]
        public string UserId { get; private set; }

        [DataMember]
        public string UserName { get; private set; }

        [DataMember]
        public string City { get; private set; }

        [DataMember]
        public string Street { get; private set; }

        [DataMember]
        public string State { get; private set; }

        [DataMember]
        public string Country { get; private set; }

        [DataMember]
        public string ZipCode { get; private set; }

        [DataMember]
        public string CardNumber { get; private set; }

        [DataMember]
        public string CardHolderName { get; private set; }

        [DataMember]
        public DateTime CardExpiration { get; private set; }

        [DataMember]
        public string CardSecurityNumber { get; private set; }

        [DataMember]
        public int CardTypeId { get; private set; }

        [DataMember]
        public IEnumerable<OrderItemDto> OrderItems => _orderItems;
    }
}
