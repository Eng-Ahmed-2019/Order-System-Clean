using MediatR;
using OrderSystem.Domain.Enums;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            Serilog.Log.Information(
                "Creating new order {@Order}",
                request.order
            );
            if (request == null) throw new ArgumentNullException(nameof(request));
            var order = new Order
            {
                UserId = request.userId,
                TotalAmount = request.order.TotalAmount,
                Status = OrderStatus.PaymentPending
            };
            Serilog.Log.Information(
                $"Order created successfully"
            );
            return await _orderRepository.CreateAsync(order);
        }
    }
}