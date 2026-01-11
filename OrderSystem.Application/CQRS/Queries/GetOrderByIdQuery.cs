using MediatR;
using OrderSystem.Application.DTOs;

namespace OrderSystem.Application.CQRS.Queries
{
    public record GetOrderByIdQuery(int id, int UserId) : IRequest<OrderResponseDto>;

    /*
    public record GetOrderByIdQuery
    {
        public int Id { get; init; }

        public GetOrderByIdQuery(int id)
        {
            Id = id;
        }
    }
    */
}