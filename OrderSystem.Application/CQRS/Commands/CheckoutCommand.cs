using MediatR;

namespace OrderSystem.Application.CQRS.Commands
{
    public record CheckoutCommand(int UserId) : IRequest;
}