using MediatR;
using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.CQRS.Queries
{
    public record GetCartQuery(int UserId) : IRequest<IEnumerable<OrderItem>>;
}