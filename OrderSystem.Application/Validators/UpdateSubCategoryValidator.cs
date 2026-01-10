using FluentValidation;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.Validators
{
    public class UpdateSubCategoryValidator : AbstractValidator<UpdateSubCategoryRequestDto>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateSubCategoryValidator(ISubCategoryRepository subCategoryRepository, ICategoryRepository categoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Category name was required")
                .MaximumLength(100)
                .WithMessage("Category name must be less than 100 characters")
                .Must(name => name.Trim().ToLower() != "string")
                .WithMessage($"Name: \"string\" is not valid")
                .MustAsync(async (name, _) => !await _subCategoryRepository.ExistsByNameAsync(name))
                .WithMessage("Category name already exists")
                .Must(name => name.Trim().Length >= 3)
                .WithMessage("Category name must be at least 3 characters");

            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("CategoryId is required here")
                .MustAsync(async (id, _) => await _categoryRepository.GetByIdAsync(id) != null)
                .WithMessage("This Category not found here")
                .GreaterThan(0)
                .WithMessage("Category Id must be greater than zero");
        }
    }
}