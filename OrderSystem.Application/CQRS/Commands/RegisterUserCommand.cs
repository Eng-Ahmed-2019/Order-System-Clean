using MediatR;

namespace OrderSystem.Application.CQRS.Commands
{
    public record RegisterUserCommand(
        string FullName,
        string NationaId,
        string Password,
        string Email
    ) : IRequest;
}