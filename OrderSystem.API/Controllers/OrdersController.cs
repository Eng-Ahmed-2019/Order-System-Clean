using MediatR;
using FluentValidation;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.CQRS.Queries;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<CreateOrderRequestDto> _validator;

        public OrdersController(IMediator mediator, IValidator<CreateOrderRequestDto> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult>CreateOrder(CreateOrderRequestDto dto, [FromServices] IValidator<CreateOrderRequestDto> validator)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                throw new NotFoundException("User not authenticated");
            var userId = int.Parse(userIdClaim);
            var id = await _mediator.Send(new CreateOrderCommand(dto, userId));
            return Ok(new { OrderId = id });
        }

        [HttpGet("get-order/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _mediator.Send(new GetOrderByIdQuery(id));
            if (order == null) return NotFound($"Not found order match with: {id}");
            return Ok(order);
        }
    }
}