using System;
using Ordering.Domain.AggregatesModel.Builders;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.Events;
using Ordering.Domain.Exceptions;
using Xunit;

namespace Ordering.UnitTests
{
    public class OrderAggregateTests
    {
        [Fact]
        public void Create_OrderItem_Success()
        {
            // Arrange
            var productId = 1;
            var productName = "FakeProductName";
            var unitPrice = 12;
            var discount = 15;
            var pictureUrl = "FakeUrl";
            var units = 5;

            // Act
            var orderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);

            // Assert
            Assert.NotNull(orderItem);
        }

        [Fact]
        public void Invalid_number_of_units()
        {
            // Arrange    
            var productId = 1;
            var productName = "FakeProductName";
            var unitPrice = 12;
            var discount = 15;
            var pictureUrl = "FakeUrl";
            var units = -1;

            // Act - Assert
            Assert.Throws<OrderingDomainException>(() => new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units));
        }

        [Fact]
        public void Invalid_total_of_order_item_lower_than_discount_applied()
        {
            // Arrange    
            var productId = 1;
            var productName = "FakeProductName";
            var unitPrice = 12;
            var discount = 15;
            var pictureUrl = "FakeUrl";
            var units = 1;

            // Act - Assert
            Assert.Throws<OrderingDomainException>(() => new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units));
        }

        [Fact]
        public void Invalid_discount_setting()
        {
            // Arrange    
            var productId = 1;
            var productName = "FakeProductName";
            var unitPrice = 12;
            var discount = 15;
            var pictureUrl = "FakeUrl";
            var units = 5;

            // Act 
            var fakeOrderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);

            // Assert
            Assert.Throws<OrderingDomainException>(() => fakeOrderItem.SetNewDiscount(-1));
        }

        [Fact]
        public void Invalid_units_setting()
        {
            // Arrange    
            var productId = 1;
            var productName = "FakeProductName";
            var unitPrice = 12;
            var discount = 15;
            var pictureUrl = "FakeUrl";
            var units = 5;

            // Act 
            var fakeOrderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);

            // Assert
            Assert.Throws<OrderingDomainException>(() => fakeOrderItem.AddUnits(-1));
        }

        [Fact]
        public void when_add_two_times_on_the_same_item_then_the_total_of_order_should_be_the_sum_of_the_two_items()
        {
            // Arrange
            var address = AddressBuilder.Build();

            // Act
            var order = new OrderBuilder(address)
                .AddOne(1, "cup", 10.0m, 0, string.Empty)
                .AddOne(1, "cup", 10.0m, 0, string.Empty)
                .Build();

            // Assert
            Assert.Equal(20.0m, order.GetTotal());
        }

        [Fact]
        public void Add_new_Order_raises_new_event()
        {
            // Arrange
            var street = "fakeStreet";
            var city = "FakeCity";
            var state = "fakeState";
            var country = "fakeCountry";
            var zipcode = "FakeZipCode";
            var cardTypeId = 5;
            var cardNumber = "12";
            var cardSecurityNumber = "123";
            var cardHolderName = "FakeName";
            var cardExpiration = DateTime.Now.AddYears(1);            
            var address = new Address(street, city, state, country, zipcode);
            var expectedResult = 1;

            // Act 
            var order = new Order("1", "fakeName", address, cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);

            // Assert
            Assert.Equal(order.DomainEvents.Count, expectedResult);
        }

        [Fact]
        public void Add_event_Order_explicitly_raises_new_event()
        {
            // Arrange   
            var street = "fakeStreet";
            var city = "FakeCity";
            var state = "fakeState";
            var country = "fakeCountry";
            var zipcode = "FakeZipCode";
            var cardTypeId = 5;
            var cardNumber = "12";
            var cardSecurityNumber = "123";
            var cardHolderName = "FakeName";
            var cardExpiration = DateTime.Now.AddYears(1);
            var address = new Address(street, city, state, country, zipcode);
            
            var expectedResult = 2;

            // Act                         
            var order = new Order("1", "fakeName", address, cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);
            var @event = new OrderStartedDomainEvent(order, "fakeName", "1", cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);
            order.AddDomainEvent(@event);
            
            // Assert
            Assert.Equal(order.DomainEvents.Count, expectedResult);
        }

        [Fact]
        public void Remove_event_Order_explicitly()
        {
            // Arrange    
            var street = "fakeStreet";
            var city = "FakeCity";
            var state = "fakeState";
            var country = "fakeCountry";
            var zipcode = "FakeZipCode";
            var cardTypeId = 5;
            var cardNumber = "12";
            var cardSecurityNumber = "123";
            var cardHolderName = "FakeName";
            var cardExpiration = DateTime.Now.AddYears(1);
            var address = new Address(street, city, state, country, zipcode);
            var order = new Order("1", "fakeName", address, cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);
            var @event = new OrderStartedDomainEvent(order, "1", "fakeName", cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);
            var expectedResult = 1;

            // Act         
            order.AddDomainEvent(@event);
            order.RemoveDomainEvent(@event);
            
            // Assert
            Assert.Equal(order.DomainEvents.Count, expectedResult);
        }
    }
}
