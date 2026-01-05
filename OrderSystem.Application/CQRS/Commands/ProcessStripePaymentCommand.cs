using MediatR;

namespace OrderSystem.Application.CQRS.Commands
{
    public record ProcessStripePaymentCommand(int orderId,decimal amount)
        :IRequest<bool>
    ;
}