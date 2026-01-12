using MediatR;

namespace OrderSystem.Application.CQRS.Queries
{
    public record RemoveFromCartQuery(int userId, int productId) : IRequest;
}