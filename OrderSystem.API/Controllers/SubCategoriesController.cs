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
    public class SubCategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<CreateSubCategoryRequestDto> _validator;
        private readonly IValidator<UpdateSubCategoryRequestDto> _validator1;
        private readonly IValidator<DeleteSubCategoryRequestDto> _validator2;

        public SubCategoriesController(
            IMediator mediator,
            IValidator<CreateSubCategoryRequestDto> validator,
            IValidator<UpdateSubCategoryRequestDto> validator1,
            IValidator<DeleteSubCategoryRequestDto> validator2)
        {
            _mediator = mediator;
            _validator = validator;
            _validator1 = validator1;
            _validator2 = validator2;
        }

        [Authorize(Roles = "Admin")]
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

        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllSubCategoriesQuery());
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update-SubCategory")]
        public async Task<IActionResult>UpdateSubCategory(UpdateSubCategoryRequestDto dto)
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
            var r = await _mediator.Send(new UpdateSubCategoryCommand(dto));
            if (!r) return BadRequest("Updated was failed");
            // return Ok(new { Updated = result });
            return Ok("Updated done successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete-SubCategory")]
        public async Task<IActionResult>DeleteSubCategory(DeleteSubCategoryRequestDto dto)
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

            var r = await _mediator.Send(new DeleteSubCategoryCommand(dto.Id));
            if (!r) return BadRequest("Deleted was failed");
            return Ok("Deleted done successfully");
        }
    }
}