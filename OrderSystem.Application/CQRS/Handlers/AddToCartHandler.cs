using MediatR;
using OrderSystem.Domain.Enums;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class AddToCartHandler
        : IRequestHandler<AddToCartCommand>
    {
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;

        public AddToCartHandler(
            ICartRepository cartRepo,
            IProductRepository productRepo)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
        }

        public async Task Handle(
            AddToCartCommand request,
            CancellationToken cancellationToken)
        {
            var cart = await _cartRepo.GetUserCartAsync(request.userId);
            if (cart == null)
            {
                cart = new Order
                {
                    UserId = request.userId,
                    Status = OrderStatus.Cart,
                    TotalAmount = 0
                };
                cart.Id = await _cartRepo.CreateCartAsync(cart);
            }

            var product = await _productRepo.GetByIdAsync(request.dto.ProductId)
                ?? throw new NotFoundException("Product not found");
            if (!product.IsActive)
                throw new Exception("Product is not available");
            if (product.Stock < request.dto.Quantity)
                throw new Exception("Not enough stock available");
            if (await _cartRepo.ItemExistsAsync(cart.Id, product.Id))
            {
                await _cartRepo.UpdateItemQuantityAsync(
                    cart.Id,
                    product.Id,
                    request.dto.Quantity
                );
            }
            else
            {
                await _cartRepo.AddItemAsync(new OrderItem
                {
                    OrderId = cart.Id,
                    ProductId = product.Id,
                    Quantity = request.dto.Quantity,
                    UnitPrice = product.Price
                });
            }
            await _cartRepo.UpdateOrderTotalAsync(cart.Id);
        }
    }
}