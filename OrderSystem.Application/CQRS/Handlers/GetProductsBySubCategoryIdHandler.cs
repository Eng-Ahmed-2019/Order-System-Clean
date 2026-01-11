using MediatR;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.CQRS.Queries;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class GetProductsBySubCategoryIdHandler :
        IRequestHandler<GetProductsBySubCategoryIdQuery, IEnumerable<Product>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ISubCategoryRepository _subCategoryRepository;

        public GetProductsBySubCategoryIdHandler(
            IProductRepository productRepository,
            ISubCategoryRepository subCategoryRepository)
        {
            _productRepository = productRepository;
            _subCategoryRepository = subCategoryRepository;
        }

        public async Task<IEnumerable<Product>> Handle(
        GetProductsBySubCategoryIdQuery request,
        CancellationToken cancellationToken)
        {
            var subCategory = await _subCategoryRepository.GetByIdAsync(request.SubCategoryId);

            if (subCategory == null) throw new BusinessException("SubCategory not found");

            return await _productRepository.GetBySubCategoryIdAsync(request.SubCategoryId);
        }
    }
}