using MediatR;

namespace OrderSystem.Application.CQRS.Commands
{
    public record ProcessPaymentCommand(int orderId) : IRequest<bool>;

    /*
    public class ProcessPaymentCommand
    {
        public int OrderId { get; init; }
        public decimal Amount { get; init; }

        public ProcessPaymentCommand(int orderId, decimal amount)
        {
            OrderId = orderId;
            Amount = amount;
        }
    }
    */
}