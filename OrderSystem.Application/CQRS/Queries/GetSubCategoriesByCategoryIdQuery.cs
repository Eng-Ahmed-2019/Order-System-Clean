using MediatR;
using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.CQRS.Queries
{
    public record GetSubCategoriesByCategoryIdQuery(int CategoryId) : IRequest<IEnumerable<SubCategory>>;
}