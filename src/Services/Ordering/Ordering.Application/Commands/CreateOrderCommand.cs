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
    [DataContract]
    public class CreateOrderCommand : IRequest<bool>
    {
        public CreateOrderCommand()
        {
            OrderItems = new List<OrderItemDto>();
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
            OrderItems = basketItems.ToOrderItemsDto().ToList();
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
        public string UserId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string Street { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public string CardHolderName { get; set; }

        [DataMember]
        public DateTime CardExpiration { get; set; }

        [DataMember]
        public string CardSecurityNumber { get; set; }

        [DataMember]
        public int CardTypeId { get; set; }

        [DataMember]
        public List<OrderItemDto> OrderItems { get; set; }

    }
}
