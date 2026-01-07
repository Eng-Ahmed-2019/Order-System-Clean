using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<CreateProductRequestDto> _validator;

        public ProductsController(
            IMediator mediator,
            IValidator<CreateProductRequestDto> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        [HttpPost("create-product")]
        public async Task<IActionResult> CreateProduct(
            CreateProductRequestDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            var id = await _mediator.Send(
                new CreateProductCommand(dto)
            );

            return Ok(new { ProductId = id });
        }
    }
}