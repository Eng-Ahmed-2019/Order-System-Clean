using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using OrderSystem.Application.CQRS.Queries;

namespace OrderSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<GetOrderDto> _validator;

        public OrdersController(IMediator mediator, IValidator<GetOrderDto> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var dto = new GetOrderDto { Id = id };

            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var order = await _mediator.Send(new GetOrderByIdQuery(id));
            if (order == null)
                return NotFound($"Not found order match with: {id}");
            return Ok(order);
        }
    }
}