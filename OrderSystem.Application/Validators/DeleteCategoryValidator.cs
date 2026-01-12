using FluentValidation;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.Validators
{
    public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;

        public DeleteCategoryValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(c => c.CategoryId)
                .NotEmpty()
                .WithMessage("CategoryId is required here")
                .GreaterThan(0)
                .WithMessage("CategoryId must be greater than zero")
                .MustAsync(async (id, _) => _categoryRepository.GetByIdAsync(id) != null)
                .WithMessage("This category not found here");
        }
    }
}