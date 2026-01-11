using MediatR;

namespace OrderSystem.Application.CQRS.Commands
{
    public record ProcessStripePaymentCommand(int orderId, int userId)
        :IRequest<bool>
    ;
}