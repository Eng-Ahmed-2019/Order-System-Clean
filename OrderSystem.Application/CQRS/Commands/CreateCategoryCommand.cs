using MediatR;
using OrderSystem.Application.DTOs;

namespace OrderSystem.Application.CQRS.Commands
{
    public record CreateCategoryCommand(CreateCategoryRequestDto dto) : IRequest<int>;
}