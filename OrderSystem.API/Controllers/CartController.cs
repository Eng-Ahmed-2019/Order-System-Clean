using MediatR;
using FluentValidation;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Application.DTOs;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using OrderSystem.Application.CQRS.Queries;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    [EnableRateLimiting("GeneralPolicy")]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<AddToCartRequestDto> _validator;
        private readonly IValidator<GetProductForCartDto> _validator1;

        public CartController(
            IMediator mediator,
            IValidator<AddToCartRequestDto> validator,
            IValidator<GetProductForCartDto> validator1)
        {
            _mediator = mediator;
            _validator = validator;
            _validator1 = validator1;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddToCart(AddToCartRequestDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("User not found");
            var userId = int.Parse(userIdClaim);

            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            await _mediator.Send(
                new AddToCartCommand(dto, userId)
            );

            return Ok("Item added to cart");
        }

        [HttpGet("Get-Items")]
        public async Task<IActionResult> GetCartItems()
        {
            var userClaims = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userClaims == null) return Unauthorized("User not found");
            var userId = int.Parse(userClaims);
            var oi = await _mediator.Send(new GetCartQuery(userId));
            if (!oi.Any()) return NotFound("Not found any item here yet");
            return Ok(oi);
        }

        [HttpDelete("Remove-Item")]
        public async Task<IActionResult> RemoveFromCart(GetProductForCartDto dto)
        {
            var validationResult = await _validator1.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("User not found");
            var userId = int.Parse(userIdClaim);
            var r = await _mediator.Send(new RemoveFromCartQuery(dto.Id));
            if (!r) return BadRequest("This item not found here");
            return Ok("Item removed from cart");
        }
    }
}