using MediatR;
using OrderSystem.Application.DTOs;

namespace OrderSystem.Application.CQRS.Commands
{
    public record CreateProductCommand(
        CreateProductRequestDto dto
    ) : IRequest<int>;
}