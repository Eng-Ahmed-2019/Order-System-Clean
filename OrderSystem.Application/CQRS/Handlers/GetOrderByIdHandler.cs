using MediatR;
using Serilog;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Queries;

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
                throw new OrderException("This request was empty");
            }
            if (request.id <= 0) throw new BusinessException("Id must be greater than zero");
            var order = await _orderRepository.GetByIdAsync(request.id, request.UserId);
            if (order == null)
            {
                Log.Warning("Order not found with Id {OrderId}", request.id);
                throw new OrderException("Order not found here");
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