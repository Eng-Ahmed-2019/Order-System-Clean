using FluentValidation;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.Validators
{
    public class CreateSubCategoryValidator : AbstractValidator<CreateSubCategoryRequestDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubCategoryRepository _subCategoryRepository;

        public CreateSubCategoryValidator(ICategoryRepository categoryRepository, ISubCategoryRepository subCategoryRepository)
        {
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository;

            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage("CategoryId must be greater than zero")
                .MustAsync(async (id, _) => await _categoryRepository.GetByIdAsync(id) != null)
                .WithMessage("Category not found");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Sub category name is required")
                .Must(name => name.Trim().ToLower() != "string")
                .WithMessage($"Name: \"string\" is not valid")
                .MustAsync(async (dto, name, _) =>
                    !await _subCategoryRepository.ExistsByNameAsync(name, dto.CategoryId))
                .WithMessage("SubCategory already exists in this category")
                .Must(name => name.Trim().Length >= 2)
                .WithMessage("Category name must be at least 3 characters");
        }
    }
}