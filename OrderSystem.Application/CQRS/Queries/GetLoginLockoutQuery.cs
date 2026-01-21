using MediatR;
using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.CQRS.Queries
{
    public record GetLoginLockoutQuery(string Key)
    : IRequest<LoginLockout?>;
}