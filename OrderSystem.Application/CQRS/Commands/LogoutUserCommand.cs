using MediatR;

namespace OrderSystem.Application.CQRS.Commands
{
    public record LogoutUserCommand(Guid sessionId):IRequest;
}