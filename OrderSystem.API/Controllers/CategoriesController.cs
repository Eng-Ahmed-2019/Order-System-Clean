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
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<CreateCategoryRequestDto> _validator;

        public CategoriesController(
            IMediator mediator,
            IValidator<CreateCategoryRequestDto> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        [HttpPost("create-category")]
        public async Task<IActionResult> CreateCategory(CreateCategoryRequestDto dto)
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

            var id = await _mediator.Send(new CreateCategoryCommand(dto));
            return Ok(new { CategoryId = id });
        }
    }
}