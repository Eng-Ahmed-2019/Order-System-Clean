using FluentValidation;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.Validators
{
    public class DeleteSubCategoryValidator : AbstractValidator<DeleteSubCategoryRequestDto>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public DeleteSubCategoryValidator(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;

            RuleFor(s => s.Id)
                .NotEmpty()
                .WithMessage("Id is required here")
                .GreaterThan(0)
                .WithMessage("Id must be greater than zero")
                .MustAsync(async (id, _) => await _subCategoryRepository.GetByIdAsync(id) != null)
                .WithMessage("SubCategory not found here");
        }
    }
}