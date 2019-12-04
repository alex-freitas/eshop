namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ordering.Domain.Events;
    using Ordering.Domain.Exceptions;
    using Ordering.Domain.SharedKernel;

    public class Order : Entity, IAggregateRoot
    {
        private readonly DateTime _orderDate;
        private readonly List<OrderItem> _orderItems;

        private int _orderStatusId;
        private string _description;

        public Order(
            string userId,
            string userName,
            Address address,
            int cardTypeId,
            string cardNumber,
            string cardSecurityNumber,
            string cardHolderName,
            DateTime cardExpiration,
            int? buyerId = null,
            int? paymentMethodId = null)
            : this()
        {
            Address = address;
            BuyerId = buyerId;
            PaymentMethodId = paymentMethodId;

            _orderStatusId = OrderStatus.Submitted.Id;
            _orderDate = DateTime.UtcNow;

            AddOrderStartedDomainEvent(userId, userName, cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);
        }

        protected Order()
        {
            _orderItems = new List<OrderItem>();
        }

        public int? PaymentMethodId { get; private set; }

        public Address Address { get; private set; }

        public int? BuyerId { get; private set; }

        public OrderStatus OrderStatus { get; private set; }

        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

        public void AddOrderItem(int productId, string productName, decimal unitPrice, decimal discount, string pictureUrl, int units = 1)
        {
            var existingOrderForProduct = _orderItems.SingleOrDefault(o => o.ProductId == productId);

            if (existingOrderForProduct != null)
            {
                // if previous line exist modify it with higher discount  and units...
                if (discount > existingOrderForProduct.GetCurrentDiscount())
                {
                    existingOrderForProduct.SetNewDiscount(discount);
                }

                existingOrderForProduct.AddUnits(units);
            }
            else
            {
                // add validated new order item
                var orderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
                _orderItems.Add(orderItem);
            }
        }

        public void SetPaymentId(int id)
        {
            PaymentMethodId = id;
        }

        public void SetBuyerId(int id)
        {
            BuyerId = id;
        }

        public decimal GetTotal()
        {
            return _orderItems.Sum(o => o.GetUnits() * o.GetUnitPrice());
        }

        #region Status
        public void SetAwaitingValidationStatus()
        {
            if (_orderStatusId == OrderStatus.Submitted.Id)
            {
                AddDomainEvent(new OrderStatusChangedToAwaitingValidationDomainEvent(Id, _orderItems));
                _orderStatusId = OrderStatus.AwaitingValidation.Id;
            }
        }

        public void SetStockConfirmedStatus()
        {
            if (_orderStatusId == OrderStatus.AwaitingValidation.Id)
            {
                AddDomainEvent(new OrderStatusChangedToStockConfirmedDomainEvent(Id));

                _orderStatusId = OrderStatus.StockConfirmed.Id;
                _description = "All the items were confirmed with available stock.";
            }
        }

        public void SetPaidStatus()
        {
            if (_orderStatusId == OrderStatus.StockConfirmed.Id)
            {
                AddDomainEvent(new OrderStatusChangedToPaidDomainEvent(Id, OrderItems));

                _orderStatusId = OrderStatus.Paid.Id;
                _description = "The payment was performed at a simulated \"American Bank checking bank account ending on XX35071\"";
            }
        }

        public void SetShippedStatus()
        {
            if (_orderStatusId != OrderStatus.Paid.Id)
            {
                StatusChangeException(OrderStatus.Shipped);
            }

            _orderStatusId = OrderStatus.Shipped.Id;
            _description = "The order was shipped.";
            AddDomainEvent(new OrderShippedDomainEvent(this));
        }

        public void SetCancelledStatus()
        {
            if (_orderStatusId == OrderStatus.Paid.Id ||
                _orderStatusId == OrderStatus.Shipped.Id)
            {
                StatusChangeException(OrderStatus.Cancelled);
            }

            _orderStatusId = OrderStatus.Cancelled.Id;
            _description = $"The order was cancelled.";
            AddDomainEvent(new OrderCancelledDomainEvent(this));
        }

        public void SetCancelledStatusWhenStockIsRejected(IEnumerable<int> orderStockRejectedItems)
        {
            if (_orderStatusId == OrderStatus.AwaitingValidation.Id)
            {
                _orderStatusId = OrderStatus.Cancelled.Id;

                var itemsStockRejectedProductNames = OrderItems
                    .Where(c => orderStockRejectedItems.Contains(c.ProductId))
                    .Select(c => c.GetOrderItemProductName());

                var itemsStockRejectedDescription = string.Join(", ", itemsStockRejectedProductNames);
                _description = $"The product items don't have stock: ({itemsStockRejectedDescription}).";
            }
        }
        #endregion

        private void AddOrderStartedDomainEvent(
            string userId,
            string userName,
            int cardTypeId,
            string cardNumber,
            string cardSecurityNumber,
            string cardHolderName,
            DateTime cardExpiration)
        {
            var evt = new OrderStartedDomainEvent(this, userId, userName, cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);

            AddDomainEvent(evt);
        }

        private void StatusChangeException(OrderStatus orderStatusToChange)
        {
            throw new OrderingDomainException($"Is not possible to change the order status from {OrderStatus.Name} to {orderStatusToChange.Name}.");
        }
    }
}
