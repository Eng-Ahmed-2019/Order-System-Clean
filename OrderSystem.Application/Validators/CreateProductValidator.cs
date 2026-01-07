using FluentValidation;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.Validators
{
    public class CreateProductValidator : AbstractValidator<CreateProductRequestDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly ISubCategoryRepository _subCategoryRepository;

        public CreateProductValidator(IProductRepository productRepository, ISubCategoryRepository subCategoryRepository)
        {
            _productRepository = productRepository;
            _subCategoryRepository = subCategoryRepository;

            RuleFor(x => x.SubCategoryId)
                .GreaterThan(0)
                .WithMessage("SubCategoryId must be greater than zero")
                .MustAsync(async (id, _) =>
                    await _subCategoryRepository.GetByIdAsync(id) != null)
                .WithMessage("SubCategory not found");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Subcategory name is required")
                .MaximumLength(150)
                .WithMessage("Subcategory name must be less than 150 characters")
                .MustAsync(async (dto, name, _) =>
                    !await _productRepository.ExistsByNameAsync(name, dto.SubCategoryId))
                .WithMessage("Product already exists in this subcategory");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than zero");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock must be greater than or equal to zero");
        }
    }
}