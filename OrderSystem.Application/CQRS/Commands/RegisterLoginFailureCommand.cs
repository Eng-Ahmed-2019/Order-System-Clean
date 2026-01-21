using MediatR;

namespace OrderSystem.Application.CQRS.Commands
{
    public record RegisterLoginFailureCommand(string Key)
    : IRequest;
}