using MediatR;
using OrderSystem.Domain.Enums;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class CheckoutHandler : IRequestHandler<CheckoutCommand>
    {
        private readonly ICartRepository _cartRepo;
        private readonly IOrderItemRepository _orderItemRepo;
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;

        public CheckoutHandler(
            ICartRepository cartRepo,
            IOrderItemRepository orderItemRepo,
            IProductRepository productRepo,
            IOrderRepository orderRepo)
        {
            _cartRepo = cartRepo;
            _orderItemRepo = orderItemRepo;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
        }

        public async Task Handle(
            CheckoutCommand request,
            CancellationToken cancellationToken)
        {
            var cart = await _cartRepo.GetUserCartAsync(request.UserId);
            if (cart == null)
                throw new NotFoundException("Cart not found");

            var items = await _orderItemRepo.GetByOrderIdAsync(cart.Id);
            if (!items.Any())
                throw new CartException("Cart is empty");

            foreach (var item in items)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId)
                    ?? throw new NotFoundException("Product not found");

                if (!product.IsActive)
                    throw new IsActiveException("Product is not available");

                if (product.Stock < item.Quantity)
                    throw new StockException($"Not enough stock for product {product.Name}");
            }
            await _orderRepo.UpdateStatusAsync(
                cart.Id,
                OrderStatus.PaymentPending
            );
        }
    }
}