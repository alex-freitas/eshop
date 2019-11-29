using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Commands;

namespace Ordering.WebApi.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Route("create")]
        [HttpPost]
        public async Task<ActionResult<bool>> CreateOrder([FromBody] CreateOrderCommand cmd)
        {
            return await _mediator.Send(cmd);
        }
    }
}
