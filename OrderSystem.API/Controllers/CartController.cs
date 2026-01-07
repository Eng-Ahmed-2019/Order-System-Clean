using MediatR;
using FluentValidation;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<AddToCartRequestDto> _validator;

        public CartController(
            IMediator mediator,
            IValidator<AddToCartRequestDto> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(
            AddToCartRequestDto dto)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _mediator.Send(
                new AddToCartCommand(dto, userId)
            );

            return Ok("Item added to cart");
        }
    }
}