using System;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.Exceptions;
using Xunit;

namespace Ordering.UnitTests
{
    /* [UnitOfWork_StateUnderTest_ExpectedBehavior] https:// osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html */

    public class BuyerAggregateTests
    {
        [Fact]
        public void CreateBuyer_WithValidParams_Success()
        {
            // Arrange    
            var identity = Guid.NewGuid().ToString();
            var name = "fakeUser";

            // Act 
            var buyer = new Buyer(identity, name);

            // Assert
            Assert.NotNull(buyer);
        }

        [Fact]
        public void CreateBuyer_WithInvalidIdentity_ExceptionThrown()
        {
            // Arrange    
            var identity = string.Empty;
            var name = "fakeUser";

            // Act - Assert
            Assert.Throws<ArgumentNullException>(() => new Buyer(identity, name));
        }

        [Fact]
        public void AddPaymentMethod_WithValidParams_Success()
        {
            // Arrange    
            var cardTypeId = 1;
            var alias = "fakeAlias";
            var cardNumber = "124";
            var securityNumber = "1234";
            var cardHolderName = "FakeHolderNAme";
            var expiration = DateTime.Now.AddYears(1);
            var orderId = 1;
            var name = "fakeUser";
            var identity = Guid.NewGuid().ToString();
            var buyerItem = new Buyer(identity, name);

            // Act
            var result = buyerItem.VerifyOrAddPaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration, orderId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void CreatePaymentMethod_WithValidParams_Success()
        {
            // Arrange    
            var cardTypeId = 1;
            var alias = "fakeAlias";
            var cardNumber = "124";
            var securityNumber = "1234";
            var cardHolderName = "FakeHolderNAme";
            var expiration = DateTime.Now.AddYears(1);

            // Act
            var result = new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void CreatePaymentMethod_WithInvalidExpiration_ExceptionThrown()
        {
            // Arrange    
            var cardTypeId = 1;
            var alias = "fakeAlias";
            var cardNumber = "124";
            var securityNumber = "1234";
            var cardHolderName = "FakeHolderNAme";
            var expiration = DateTime.Now.AddYears(-1);

            // Act - Assert
            Assert.Throws<OrderingDomainException>(() => new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration));
        }

        [Fact]
        public void ComparePaymentMethod_WithSameInfo_Success()
        {
            // Arrange    
            var cardTypeId = 1;
            var alias = "fakeAlias";
            var cardNumber = "124";
            var securityNumber = "1234";
            var cardHolderName = "FakeHolderNAme";
            var expiration = DateTime.Now.AddYears(1);

            // Act
            var paymentMethod = new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);
            var result = paymentMethod.IsEqualTo(cardTypeId, cardNumber, expiration);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AddNewPaymentMethod_WithValidParams_RaisesNewEvent()
        {
            // Arrange    
            var alias = "fakeAlias";
            var orderId = 1;
            var cardTypeId = 5;
            var cardNumber = "12";
            var cardSecurityNumber = "123";
            var cardHolderName = "FakeName";
            var cardExpiration = DateTime.Now.AddYears(1);
            var expectedResult = 1;
            var name = "fakeUser";

            // Act 
            var buyer = new Buyer(Guid.NewGuid().ToString(), name);
            buyer.VerifyOrAddPaymentMethod(cardTypeId, alias, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration, orderId);

            // Assert
            Assert.Equal(buyer.DomainEvents.Count, expectedResult);
        }
    }
}
