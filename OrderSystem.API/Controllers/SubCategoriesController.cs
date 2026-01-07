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
    public class SubCategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<CreateSubCategoryRequestDto> _validator;

        public SubCategoriesController(
            IMediator mediator,
            IValidator<CreateSubCategoryRequestDto> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        [HttpPost("create-subcategory")]
        public async Task<IActionResult> CreateSubCategory(CreateSubCategoryRequestDto dto)
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

            var id = await _mediator.Send(new CreateSubCategoryCommand(dto));
            return Ok(new { SubCategoryId = id });
        }
    }
}