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

            RuleFor(x => x.OrderNumber)
                .NotEmpty()
                .WithMessage("Order number is required")
                .MaximumLength(50)
                .WithMessage("Order number must not exceed 50 characters")
                .Must(NotBeDefaultValue)
                .WithMessage("Order number is invalid")
                .Matches(@"^(ORD|INV)-\d{4,}$")
                .WithMessage("Order number must follow pattern: ORD-20240001")
                .MustAsync(BeUniqueOrderNumber)
                .WithMessage("Order number already exists");

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0)
                .WithMessage("Total amount must be greater than 0");
        }

        private bool NotBeDefaultValue(string orderNumber)
        {
            var invalidValues = new[] { "string", "test", "null", "undefined" };
            return !invalidValues.Contains(orderNumber.ToLower());
        }

        private async Task<bool> BeUniqueOrderNumber(
            string orderNumber,
            CancellationToken cancellationToken)
        {
            return !await _orderRepository.ExistsByOrderNumberAsync(orderNumber);
        }
    }
}