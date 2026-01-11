using MediatR;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Queries;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class RemoveFromCartQueryHandler : IRequestHandler<RemoveFromCartQuery, bool>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ICartRepository _cartRepository;

        public RemoveFromCartQueryHandler(IOrderItemRepository orderItemRepository,ICartRepository cartRepository)
        {
            _orderItemRepository = orderItemRepository;
            _cartRepository = cartRepository;
        }

        public async Task<bool> Handle(RemoveFromCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _orderItemRepository.GetByIdAsync(request.id);
            if (cart == null) throw new NotFoundException("Cart not found");
            var item = await _orderItemRepository.GetByIdAsync(request.id);
            if (item == null) throw new NotFoundException("Cart item not found");
            await _cartRepository.RemoveItemAsync(item.Id);
            await _cartRepository.UpdateOrderTotalAsync(item.OrderId);
            var items = await _orderItemRepository.GetByOrderIdAsync(item.OrderId);
            if (!items.Any())
            {
                await _cartRepository.DeleteCartAsync(item.OrderId);
            }
            return true;
        }
    }
}