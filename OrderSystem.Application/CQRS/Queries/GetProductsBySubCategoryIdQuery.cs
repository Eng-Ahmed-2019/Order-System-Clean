using MediatR;
using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.CQRS.Queries
{
    public record GetProductsBySubCategoryIdQuery(int SubCategoryId) : IRequest<IEnumerable<Product>>;
}