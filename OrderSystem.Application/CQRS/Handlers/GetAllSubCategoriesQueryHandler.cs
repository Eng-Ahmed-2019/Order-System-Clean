using MediatR;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Queries;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class GetAllSubCategoriesQueryHandler :
        IRequestHandler<GetAllSubCategoriesQuery, IEnumerable<SubCategory>>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public GetAllSubCategoriesQueryHandler(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        public async Task<IEnumerable<SubCategory>>Handle(GetAllSubCategoriesQuery query,CancellationToken cancellationToken)
        {
            return await _subCategoryRepository.GetAllAsync();
        }
    }
}