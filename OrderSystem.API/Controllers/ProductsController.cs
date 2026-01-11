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
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<CreateProductRequestDto> _validator;
        private readonly IValidator<UpdateProductRequestDto> _validator1;
        private readonly IValidator<DeleteProductRequestDto> _validator2;
        private readonly IValidator<GetProductsInSubCategoryRequestDto> _validator3;

        public ProductsController(
            IMediator mediator,
            IValidator<CreateProductRequestDto> validator,
            IValidator<UpdateProductRequestDto> validator1,
            IValidator<DeleteProductRequestDto> validator2,
            IValidator<GetProductsInSubCategoryRequestDto> validator3)
        {
            _mediator = mediator;
            _validator = validator;
            _validator1 = validator1;
            _validator2 = validator2;
            _validator3 = validator3;
        }

        [Authorize(Roles = "Admin")]
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

        [HttpGet("Get-All-Products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _mediator.Send(new GetAllProductsQuery());
            return Ok(products);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update-Product")]
        public async Task<IActionResult> UpdateProduct(UpdateProductRequestDto dto)
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

            var r = await _mediator.Send(new UpdateProductCommand(dto));
            if (!r) return BadRequest("Updated was failed");
            return Ok("Updated done successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete-Product")]
        public async Task<IActionResult> DeleteProduct(DeleteProductRequestDto dto)
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

            var r = await _mediator.Send(new DeleteProductCommand(dto.ProductId));
            if (!r) return BadRequest("Delete was failed");
            return Ok("Deleted done successfully");
        }

        [HttpGet("Products-In-SubCategory")]
        public async Task<IActionResult> GetBySubCategory(GetProductsInSubCategoryRequestDto dto)
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
                new GetProductsBySubCategoryIdQuery(dto.SubCategoryId)
            );
            if (result == null) return NotFound("NotFound any products here yet");
            return Ok(result);
        }
    }
}