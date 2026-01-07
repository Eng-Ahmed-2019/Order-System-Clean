using FluentValidation;
using OrderSystem.Application.DTOs;

namespace OrderSystem.Application.Validators
{
    public class AddToCartValidator
        : AbstractValidator<AddToCartRequestDto>
    {
        public AddToCartValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than zero");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Qunatity must be greater than zero");
        }
    }
}