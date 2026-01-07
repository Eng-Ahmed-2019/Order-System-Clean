using MediatR;
using OrderSystem.Application.DTOs;

namespace OrderSystem.Application.CQRS.Commands
{
    public record CreateSubCategoryCommand(CreateSubCategoryRequestDto dto) : IRequest<int>;
}