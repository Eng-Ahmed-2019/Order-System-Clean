using FluentValidation;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.Validators
{
    public class DeleteProductValidator : AbstractValidator<DeleteProductRequestDto>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductValidator(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            RuleFor(p => p.ProductId)
                .NotEmpty()
                .WithMessage("Product id is required here")
                .GreaterThan(0)
                .WithMessage("Product Id must be greater than zero")
                .MustAsync(async (id, _) => await _productRepository.GetByIdAsync(id) != null)
                .WithMessage("Product not found here");
        }
    }
}