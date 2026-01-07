using MediatR;
using Serilog;
using OrderSystem.Application.CQRS.Queries;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderResponseDto>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderResponseDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            Log.Information("Fetching order with Id {OrderId}", request.id);
            if (request == null)
            {
                Log.Warning("This request was empty");
                throw new ArgumentNullException(nameof(request));
            }
            var order = await _orderRepository.GetByIdAsync(request.id);
            if (order == null)
            {
                Log.Warning("Order not found with Id {OrderId}", request.id);
                throw new ArgumentNullException(nameof(order));
            }
            return new OrderResponseDto
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt
            };
        }
    }
}