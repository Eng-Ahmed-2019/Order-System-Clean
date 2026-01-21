using MediatR;

namespace OrderSystem.Application.CQRS.Commands
{
    public record ResetLoginLockoutCommand(string Key)
    : IRequest;
}