using FluentValidation;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.Validators
{
    public class AddToCartValidator
        : AbstractValidator<AddToCartRequestDto>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public AddToCartValidator(ICartRepository cartRepository ,IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;

            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than zero")
                .MustAsync(async (id, _) => await _productRepository.GetByIdAsync(id) != null)
                .WithMessage($"Product not found");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Qunatity must be greater than zero");
        }
    }
}