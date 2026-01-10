using MediatR;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Queries;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<Product>>
    {
        private readonly IProductRepository _repository;

        public GetAllProductsQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Product>>Handle(GetAllProductsQuery query,CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync();
        }
    }
}