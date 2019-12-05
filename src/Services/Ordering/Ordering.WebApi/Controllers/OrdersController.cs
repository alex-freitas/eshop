using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Commands;
using Ordering.Application.Models;
using Ordering.Application.Queries;
using Ordering.WebApi.Extensions;
using Ordering.WebApi.Infrastructure.ActionResults;

namespace Ordering.WebApi.Controllers
{
    /// <summary>
    /// Everything about Orders
    /// </summary>    
    [Produces("application/json")]
    [Route("orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IOrderQueries _orderQueries;

        public OrdersController(IMediator mediator, IOrderQueries orderQueries)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
        }

        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Order>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetOrdersAsync()
        {
            try
            {
                var order = await _orderQueries.GetOrdersAsync();

                return Ok(order);
            }
            catch (Exception ex)
            {
                return new InternalServerErrorObjectResult(ex.Message);
            }
        }

        /// <summary>
        /// Creates an order.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /orders/create
        ///     {  
        ///         "userId": "1234",
        ///         "userName": "string",
        ///         "city": "string",
        ///         "street": "string",
        ///         "state": "string",
        ///         "country": "string",
        ///         "zipCode": "string",
        ///         "cardNumber": "1234-5678-0912-3456",
        ///         "cardHolderName": "string",
        ///         "cardExpiration": "2019-12-31T01:17:39.605Z",
        ///         "cardSecurityNumber": "123",
        ///         "cardTypeId": 1,
        ///         "orderItems": [
        ///           {
        ///             "productId": 1,
        ///             "productName": "string",
        ///             "unitPrice": 10,
        ///             "discount": 0,
        ///             "units": 1,
        ///             "pictureUrl": "string"
        ///           }
        ///         ]
        ///     }
        ///
        /// </remarks>
        /// <param name="command"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="200">True if success</response>
        /// <response code="400">If the item is null</response>
        /// <response code="500">Exception</response>
        [Route("create")]
        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            try
            {
                var response = await _mediator.Send(command);

                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is ValidationException validationException)
                {
                    return BadRequest(validationException.Errors);
                }

                return new InternalServerErrorObjectResult(ex.Messages());
            }
        }

        [Route("createDefault")]
        [HttpPost]
        public async Task<IActionResult> CreateDefault()
        {
            try
            {
                var items = new List<BasketItem>
                { 
                    new BasketItem { ProductId = "1", ProductName = "productName", UnitPrice = 10,  Quantity = 1, PictureUrl ="pictureUrl" }
                };

                var command = new CreateOrderCommand(
                    items,
                    "1234",
                    "userName",
                    "city",
                    "street",
                    "state",
                    "country",
                    "zipcode",
                    "1234-5678-0912-3456",
                    "holder",
                    DateTime.Now.AddYears(1),
                    "123",
                    1);

                var response = await _mediator.Send(command);

                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is ValidationException validationException)
                {
                    return BadRequest(validationException.Errors);
                }

                return new InternalServerErrorObjectResult(ex.Messages());
            }
        }
    }
}
