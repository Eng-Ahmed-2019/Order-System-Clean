using MediatR;
using OrderSystem.Application.DTOs;

namespace OrderSystem.Application.CQRS.Commands
{
    public record CreateOrderCommand(CreateOrderRequestDto order, int userId) : IRequest<int>;

    /*
    public record CreateOrderCommand
    {
        public CreateOrderRequestDto Order { get; init; }

        public CreateOrderCommand(CreateOrderRequestDto order)
        {
            Order = order;
        }
    }
    */
}