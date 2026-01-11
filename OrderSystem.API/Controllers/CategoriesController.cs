using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using OrderSystem.Application.CQRS.Queries;
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
        private readonly IValidator<UpdateCategoryRequestDto> _validator1;
        private readonly IValidator<DeleteCategoryDto> _validator2;
        private readonly IValidator<GetAllSubcategoriesRequestDto> _validator3;

        public CategoriesController(
            IMediator mediator,
            IValidator<CreateCategoryRequestDto> validator,
            IValidator<UpdateCategoryRequestDto> validator1,
            IValidator<DeleteCategoryDto> validator2,
            IValidator<GetAllSubcategoriesRequestDto> validator3
            )
        {
            _mediator = mediator;
            _validator = validator;
            _validator1 = validator1;
            _validator2 = validator2;
            _validator3 = validator3;
        }

        [Authorize(Roles = "Admin")]
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

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _mediator.Send(new GetAllCategoriesQuery());
            return Ok(categories);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update-Category")]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryRequestDto dto)
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
            var r = await _mediator.Send(new UpdateCategoryCommand(dto));
            if (!r) return BadRequest("Updated failed");
            return Ok("Updated successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete-Category")]
        public async Task<IActionResult> DeleteCategory(DeleteCategoryDto dto)
        {
            var validationResult = await _validator2.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }
            var r = await _mediator.Send(new DeleteCategoryCommand(dto.CategoryId));
            if (!r) return BadRequest("Deleted failed");
            return Ok("Deleted successfully");
        }

        [HttpGet("Certain-SubCategories")]
        public async Task<IActionResult>
            GetSubCategoriesByCategory([FromQuery] GetAllSubcategoriesRequestDto dto)
        {
            var validationResult = await _validator3.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }
            var result = await _mediator.Send(
                new GetSubCategoriesByCategoryIdQuery(dto.Id)
            );
            if (!result.Any()) return NotFound("Not found any SubCategories in this Category");

            return Ok(result);
        }
    }
}