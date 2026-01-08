using FluentValidation;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.Validators
{
    public class GetOrderValidator : AbstractValidator<GetOrderDto>
    {
        public IOrderRepository _repo;

        public GetOrderValidator(IOrderRepository repo)
        {
            _repo = repo;

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Order Id is required")
                .Must(id => id > 0)
                .WithMessage("Id must be greater than zero")
                .MustAsync(async (id, _) => await _repo.GetByIdAsync(id) != null)
                .WithMessage("Order not found here");
        }
    }
}