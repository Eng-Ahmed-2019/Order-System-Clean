using MediatR;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Queries;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, IEnumerable<OrderItem>>
    {
        private readonly ICartRepository _cartRepository;

        public GetCartQueryHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<IEnumerable<OrderItem>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetUserCartAsync(request.UserId);
            if (cart == null) return Enumerable.Empty<OrderItem>();

            var items = await _cartRepository.GetCartItemsAsync(cart.Id);
            return items;
        }
    }
}