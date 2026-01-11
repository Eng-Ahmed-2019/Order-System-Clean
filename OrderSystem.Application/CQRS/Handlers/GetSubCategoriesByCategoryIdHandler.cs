using MediatR;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Queries;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class GetSubCategoriesByCategoryIdHandler :
        IRequestHandler<GetSubCategoriesByCategoryIdQuery, IEnumerable<SubCategory>>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ICategoryRepository _categoryRepository;

        public GetSubCategoriesByCategoryIdHandler(
            ISubCategoryRepository subCategoryRepository,
            ICategoryRepository categoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<SubCategory>> Handle(
        GetSubCategoriesByCategoryIdQuery request,
        CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null) throw new BusinessException("Category not found");
            return await _subCategoryRepository.GetByCategoryIdAsync(request.CategoryId);
        }
    }
}