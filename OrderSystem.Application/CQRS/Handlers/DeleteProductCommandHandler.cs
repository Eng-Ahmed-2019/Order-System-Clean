using MediatR;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<bool>Handle(DeleteProductCommand request,CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.id);
            if (product == null) throw new BusinessException("Product not found");
            return await _productRepository.DeleteAsync(request.id);
        }
    }
}