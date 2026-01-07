using FluentValidation;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.Validators
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderRequestDto>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderValidator(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0)
                .WithMessage("Total amount must be greater than 0");
        }
    }
}