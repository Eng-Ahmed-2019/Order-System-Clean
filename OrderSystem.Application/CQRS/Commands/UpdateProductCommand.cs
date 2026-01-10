using MediatR;
using OrderSystem.Application.DTOs;

namespace OrderSystem.Application.CQRS.Commands
{
    public record UpdateProductCommand(UpdateProductRequestDto Dto) : IRequest<bool>;
}