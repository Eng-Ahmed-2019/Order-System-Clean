using MediatR;
using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.CQRS.Queries
{
    public record GetAllSubCategoriesQuery : IRequest<IEnumerable<SubCategory>>;
}