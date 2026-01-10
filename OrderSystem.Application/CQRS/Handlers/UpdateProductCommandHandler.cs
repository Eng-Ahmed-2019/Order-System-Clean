using MediatR;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<bool> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(command.Dto.Id);
            if (product == null) throw new BusinessException("Product not found");

            product.Name = command.Dto.Name;
            product.Description = command.Dto.Description;
            product.Price = command.Dto.Price;
            product.Stock = command.Dto.Stock;
            product.SubCategoryId = command.Dto.SubCategoryId;
            product.IsActive = command.Dto.Stock > 0;

            return await _productRepository.UpdateAsync(product);
        }
    }
}