using FlashSale.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlashSale.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<PlaceOrderResult>> PlaceOrder([FromBody] OrderDto orderDto)
        {
            var command = new PlaceOrderCommand(orderDto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
