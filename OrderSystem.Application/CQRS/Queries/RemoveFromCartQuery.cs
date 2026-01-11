using MediatR;

namespace OrderSystem.Application.CQRS.Queries
{
    public record RemoveFromCartQuery(int id) : IRequest<bool>;
}