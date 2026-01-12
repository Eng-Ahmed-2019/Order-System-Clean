using MediatR;

namespace OrderSystem.Application.CQRS.Commands
{
    public record DeleteCategoryCommand(int Id) : IRequest<bool>;
}