using FluentValidation;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.Validators
{
    public class GetProductForCart : AbstractValidator<GetProductForCartDto>
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public GetProductForCart(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required")
                .GreaterThan(0)
                .WithMessage("Id must be greater than zero")
                .MustAsync(async (id, _) => await _orderItemRepository.GetByIdAsync(id) != null)
                .WithMessage("This item not found here");
        }
    }
}