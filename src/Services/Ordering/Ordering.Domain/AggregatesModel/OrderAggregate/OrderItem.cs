
using Ordering.Domain.Exceptions;
using Ordering.Domain.SharedKernel;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    public class OrderItem : Entity
    {
        private readonly string _productName;
        private readonly decimal _unitPrice;
        private decimal _discount;
        private readonly string _pictureUrl;
        private int _units;

        public int ProductId { get; private set; }

        protected OrderItem() { }

        public OrderItem(int productId, string productName, decimal unitPrice, decimal discount, string pictureUrl, int units = 1)
        {
            if (units <= 0)
                throw new OrderingDomainException("Invalid number of units");

            if ((unitPrice * units) < discount)
                throw new OrderingDomainException("The total of order item is lower than applied discount");

            ProductId = productId;
            
            _productName = productName;
            _unitPrice = unitPrice;
            _discount = discount;
            _pictureUrl = pictureUrl;
            _units = units;
        }

        public string GetPictureUri() => _pictureUrl;

        public decimal GetCurrentDiscount() => _discount;

        public int GetUnits() => _units;

        public decimal GetUnitPrice() => _unitPrice;

        public string GetOrderItemProductName() => _productName;

        public void SetNewDiscount(decimal discount)
        {
            if (discount < 0)
                throw new OrderingDomainException("Discount is not valid");

            _discount = discount;
        }

        public void AddUnits(int units)
        {
            if (units < 0)
                throw new OrderingDomainException("Invalid units");

            _units += units;
        }
    }
}
