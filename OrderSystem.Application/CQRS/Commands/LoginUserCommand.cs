using MediatR;
using OrderSystem.Application.DTOs;

namespace OrderSystem.Application.CQRS.Commands
{
    public record LoginUserCommand(
        string Email,
        string password
    ) : IRequest<LoginResponseDto>;
}