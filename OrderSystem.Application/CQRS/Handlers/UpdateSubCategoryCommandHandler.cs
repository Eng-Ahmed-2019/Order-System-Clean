using MediatR;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class UpdateSubCategoryCommandHandler : IRequestHandler<UpdateSubCategoryCommand, bool>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public UpdateSubCategoryCommandHandler(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        public async Task<bool>Handle(UpdateSubCategoryCommand command,CancellationToken cancellationToken)
        {
            var subCategory = await _subCategoryRepository.GetByIdAsync(command.dto.Id);
            if (subCategory == null) throw new BusinessException("SubCategory not found");

            subCategory.Name = command.dto.Name;
            subCategory.Description = command.dto.Description;
            subCategory.CategoryId = command.dto.CategoryId;

            return await _subCategoryRepository.UpdateAsync(subCategory);
        }
    }
}