using MediatR;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Exceptions;
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
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.dto.Name.Trim().ToLower() == "string") throw new BusinessException("Invalid category name");
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