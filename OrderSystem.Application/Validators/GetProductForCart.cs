using FluentValidation;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.Validators
{
    public class GetProductForCart : AbstractValidator<GetProductForCartDto>
    {
        private readonly IProductRepository _productRepository;

        public GetProductForCart(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product id is required")
                .GreaterThan(0)
                .WithMessage("Product Id must be greater than zero")
                .MustAsync(async (id, _) => await _productRepository.GetByIdAsync(id) != null)
                .WithMessage("Product not found here");
        }
    }
}