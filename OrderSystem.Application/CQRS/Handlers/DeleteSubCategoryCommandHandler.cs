using MediatR;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class DeleteSubCategoryCommandHandler : IRequestHandler<DeleteSubCategoryCommand, bool>
    {
        private readonly ISubCategoryRepository _repository;

        public DeleteSubCategoryCommandHandler(ISubCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool>Handle(DeleteSubCategoryCommand command,CancellationToken cancellationToken)
        {
            var subcategory = await _repository.GetByIdAsync(command.id);
            if (subcategory == null) throw new BusinessException("SubCategory not found");
            return await _repository.DeleteAsync(command.id);
        }
    }
}