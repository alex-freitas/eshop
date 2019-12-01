using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Ordering.Application.Commands;
using Ordering.Application.Commands.Handlers;
using Ordering.Application.IntegrationsEvents;
using Ordering.Application.Models;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Xunit;

namespace Ordering.UnitTests.Application.Commands.Handlers
{
    public class CreateOrderCommandHandlerTests 
    {
        private readonly MockRepository _mockRepository;

        private readonly Mock<IOrderRepository> _orderRepositoryMock;       
        private readonly Mock<IOrderingIntegrationEventService> _orderingIntegrationEventServiceMock;
        private readonly Mock<ILogger<CreateOrderCommandHandler>> _loggerMock;

        public CreateOrderCommandHandlerTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);

            _orderRepositoryMock = _mockRepository.Create<IOrderRepository>();          
            _orderingIntegrationEventServiceMock = _mockRepository.Create<IOrderingIntegrationEventService>();
            _loggerMock = _mockRepository.Create<ILogger<CreateOrderCommandHandler>>();            
        }             

        [Fact]
        public async Task Handle_WithNullRequest_ReturnFalse()
        {
            // Arrange
            var handler = NewCreateOrderCommandHandler();
            CreateOrderCommand request = null;
            CancellationToken cancellationToken = default;

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Handle_IfOrderIsNotPersisted_ReturnFalse() 
        {
            // Arrange
            var handler = NewCreateOrderCommandHandler();        

            var createOrderCmd = NewCreateOrderCommand(new Dictionary<string, object>
            {
                ["cardExpiration"] = DateTime.Now.AddYears(1)
            });

            _orderRepositoryMock.Setup(x => x.UnitOfWork.SaveChangesAsync(default))
                .Returns(Task.FromResult(1));
  
            // Act
            var result = await handler.Handle(createOrderCmd, default);

            // Assert
            Assert.False(result);
        }

        private CreateOrderCommandHandler NewCreateOrderCommandHandler()
        {
            return new CreateOrderCommandHandler(
                _orderRepositoryMock.Object,                
                _orderingIntegrationEventServiceMock.Object,
                _loggerMock.Object);
        }

        private CreateOrderCommand NewCreateOrderCommand(Dictionary<string, object> args = null)
        {
            return new CreateOrderCommand(
                new List<BasketItem>(),
                userId: args != null && args.ContainsKey("userId") ? (string)args["userId"] : null,
                userName: args != null && args.ContainsKey("userName") ? (string)args["userName"] : null,
                city: args != null && args.ContainsKey("city") ? (string)args["city"] : null,
                street: args != null && args.ContainsKey("street") ? (string)args["street"] : null,
                state: args != null && args.ContainsKey("state") ? (string)args["state"] : null,
                country: args != null && args.ContainsKey("country") ? (string)args["country"] : null,
                zipcode: args != null && args.ContainsKey("zipcode") ? (string)args["zipcode"] : null,
                cardNumber: args != null && args.ContainsKey("cardNumber") ? (string)args["cardNumber"] : "1234",
                cardExpiration: args != null && args.ContainsKey("cardExpiration") ? (DateTime)args["cardExpiration"] : DateTime.MinValue,
                cardSecurityNumber: args != null && args.ContainsKey("cardSecurityNumber") ? (string)args["cardSecurityNumber"] : "123",
                cardHolderName: args != null && args.ContainsKey("cardHolderName") ? (string)args["cardHolderName"] : "XXX",
                cardTypeId: args != null && args.ContainsKey("cardTypeId") ? (int)args["cardTypeId"] : 0);
        }
    }
}
