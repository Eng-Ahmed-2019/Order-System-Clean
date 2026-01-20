using MediatR;
using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.CQRS.Queries;

public record GetMyPendingOrdersQuery(int UserId) : IRequest<IEnumerable<Order>>;