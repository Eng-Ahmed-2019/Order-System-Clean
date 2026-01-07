using FluentValidation;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.Validators
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequestDto>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Category name was required")
                .MaximumLength(100)
                .WithMessage("Category name must be less than 100 characters")
                .MustAsync(async (name, _) => !await _categoryRepository.ExistsByNameAsync(name))
                .WithMessage("Category name already exists");
        }
    }
}