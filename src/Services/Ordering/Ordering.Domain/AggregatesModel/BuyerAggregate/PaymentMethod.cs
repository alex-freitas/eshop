using System;
using Ordering.Domain.Exceptions;
using Ordering.Domain.SharedKernel;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate
{
    public class PaymentMethod : Entity
    {
        private readonly string _alias;
        private readonly string _cardNumber;
        private readonly string _securityNumber;
        private readonly string _cardHolderName;
        private readonly DateTime _expiration;
        private readonly int _cardTypeId;

        public PaymentMethod(int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
        {
            _cardNumber = !string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new OrderingDomainException(nameof(cardNumber));
            _securityNumber = !string.IsNullOrWhiteSpace(securityNumber) ? securityNumber : throw new OrderingDomainException(nameof(securityNumber));
            _cardHolderName = !string.IsNullOrWhiteSpace(cardHolderName) ? cardHolderName : throw new OrderingDomainException(nameof(cardHolderName));
            _alias = alias;
            _cardTypeId = cardTypeId;
            _expiration = (expiration >= DateTime.UtcNow) ? expiration : throw new OrderingDomainException(nameof(expiration));
        }

        protected PaymentMethod()
        {
        }

        public CardType CardType { get; private set; }

        public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
        {
            return _cardTypeId == cardTypeId && _cardNumber == cardNumber && _expiration == expiration;
        }
    }
}
