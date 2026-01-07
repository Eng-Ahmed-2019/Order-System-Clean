using MediatR;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, int>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var category = new Category
            {
                Name = request.dto.Name,
                Description = request.dto.Description,
            };
            return await _categoryRepository.CreateAsync(category);
        }
    }
}