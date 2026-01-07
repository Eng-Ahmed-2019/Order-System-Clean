using MediatR;
using OrderSystem.Application.DTOs;

namespace OrderSystem.Application.CQRS.Commands
{
    public record AddToCartCommand(
        AddToCartRequestDto dto,
        int userId
    ) : IRequest;
}