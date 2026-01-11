using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using OrderSystem.Application.CQRS.Queries;

namespace OrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("User not found");
            var userId = int.Parse(userIdClaim);

            var dto = new GetOrderDto { Id = id };

            var order = await _mediator.Send(new GetOrderByIdQuery(id, userId));
            if (order == null) return NotFound("Order not found here");

            return Ok(order);
        }
    }
}