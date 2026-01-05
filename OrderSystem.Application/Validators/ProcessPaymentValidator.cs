using FluentValidation;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.Validators
{
    public class ProcessPaymentValidator : AbstractValidator<ProcessPaymentRequestDto>
    {
        private readonly IPaymentRepository _repository;

        public ProcessPaymentValidator(IPaymentRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.OrderId)
                .GreaterThan(0)
                .WithMessage("OrderId must be greater than 0");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than 0")
                .Must(HaveMaxTwoDecimalPlaces)
                .WithMessage("Amount must have at most 2 decimal places");
        }
        private bool HaveMaxTwoDecimalPlaces(decimal amount)
        {
            return decimal.Round(amount, 2) == amount;
        }
    }
}