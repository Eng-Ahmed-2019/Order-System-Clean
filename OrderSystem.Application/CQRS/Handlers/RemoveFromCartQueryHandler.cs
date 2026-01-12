using MediatR;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Queries;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class RemoveFromCartQueryHandler : IRequestHandler<RemoveFromCartQuery>
    {
        private readonly ICartRepository _cartRepository;

        public RemoveFromCartQueryHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task Handle(RemoveFromCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetUserCartAsync(request.userId);
            if (cart == null) throw new NotFoundException("Cart not found");
            await _cartRepository.RemoveItemAsync(cart.Id, request.productId);
        }
    }
}