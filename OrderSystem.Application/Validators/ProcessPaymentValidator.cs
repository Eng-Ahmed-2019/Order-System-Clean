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
        }
    }
}