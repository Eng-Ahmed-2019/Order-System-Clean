using MediatR;
using OrderSystem.Application.DTOs;

namespace OrderSystem.Application.CQRS.Commands
{
    public record UpdateSubCategoryCommand(UpdateSubCategoryRequestDto dto) : IRequest<bool>;
}