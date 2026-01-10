using MediatR;

namespace OrderSystem.Application.CQRS.Commands
{
    public record DeleteSubCategoryCommand(int id) : IRequest<bool>;
}