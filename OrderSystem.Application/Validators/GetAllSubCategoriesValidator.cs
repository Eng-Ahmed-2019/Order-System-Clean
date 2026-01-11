using FluentValidation;
using OrderSystem.Application.DTOs;

namespace OrderSystem.Application.Validators
{
    public class GetAllSubCategoriesValidator : AbstractValidator<GetAllSubcategoriesRequestDto>
    {
        public GetAllSubCategoriesValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Category Id is required here")
                .GreaterThan(0)
                .WithMessage("Category Id must be greater than zero");
        }
    }
}