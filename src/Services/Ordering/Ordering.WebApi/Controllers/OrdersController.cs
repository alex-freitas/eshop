using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Commands;
using Ordering.Application.Queries;
using Ordering.WebApi.Infrastructure.ActionResults;

namespace Ordering.WebApi.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IOrderQueries _orderQueries;

        public OrdersController(IMediator mediator, IOrderQueries orderQueries)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
        }

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
                return new InternalServerErrorObjectResult(ex);
            }
        }

        [Route("create")]
        [HttpPost]
        public async Task<ActionResult<bool>> CreateOrder([FromBody] CreateOrderCommand cmd)
        {
            return await _mediator.Send(cmd);
        }
    }
}
