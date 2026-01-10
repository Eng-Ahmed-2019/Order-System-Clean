using MediatR;

namespace OrderSystem.Application.CQRS.Commands
{
    public record DeleteProductCommand(int id) : IRequest<bool>;
}