using MediatR;
using OrderSystem.Domain.Enums;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Queries;

namespace OrderSystem.Application.CQRS.Handlers;

public class GetMyPendingOrdersHandler : IRequestHandler<GetMyPendingOrdersQuery, IEnumerable<Order>>
{
    private readonly IOrderRepository _orderRepository;

    public GetMyPendingOrdersHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<Order>> Handle(GetMyPendingOrdersQuery request, CancellationToken cancellationToken)
    {
        var all = await _orderRepository.GetByStatusAsync(OrderStatus.PaymentPending);
        return all.Where(o => o.UserId == request.UserId);
    }
}