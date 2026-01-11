using FluentValidation;
using OrderSystem.Application.DTOs;

namespace OrderSystem.Application.Validators
{
    public class GetAllProductsValidator : AbstractValidator<GetProductsInSubCategoryRequestDto>
    {
        public GetAllProductsValidator()
        {
            RuleFor(p => p.SubCategoryId)
                .NotEmpty()
                .WithMessage("SubCategoryId is required here")
                .GreaterThan(0)
                .WithMessage("SubCategoryId must be greater than zero");
        }
    }
}