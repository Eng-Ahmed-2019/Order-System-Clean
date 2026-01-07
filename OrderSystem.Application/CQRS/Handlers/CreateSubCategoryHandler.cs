using MediatR;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class CreateSubCategoryHandler : IRequestHandler<CreateSubCategoryCommand, int>
    {
        private readonly ISubCategoryRepository _repository;

        public CreateSubCategoryHandler(ISubCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateSubCategoryCommand request, CancellationToken cancellationToken)
        {
            var subCategory = new SubCategory
            {
                CategoryId = request.dto.CategoryId,
                Name = request.dto.Name,
                Description = request.dto.Description
            };

            return await _repository.CreateAsync(subCategory);
        }
    }
}