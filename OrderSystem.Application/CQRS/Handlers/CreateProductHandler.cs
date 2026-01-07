using MediatR;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class CreateProductHandler
        : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IProductRepository _repository;

        public CreateProductHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            var product = new Product
            {
                SubCategoryId = request.dto.SubCategoryId,
                Name = request.dto.Name,
                Description = request.dto.Description,
                Price = request.dto.Price,
                Stock = request.dto.Stock,
                IsActive = true
            };
            return await _repository.CreateAsync(product);
        }
    }
}