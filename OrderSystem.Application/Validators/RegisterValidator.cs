using FluentValidation;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Application.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterRequestDto>
    {
        private readonly IUserRepository _userRepository;

        public RegisterValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required")
                .MaximumLength(50).WithMessage("FullName must be less than 50 characters");
                
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email format is not valid")
                .MustAsync(async (email, cancellation) => !await _userRepository.ExistsByEmailAsync(email))
                .WithMessage("Email already exists");

            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("NationalId is required")
                .Must(NationalIdValidator.IsValid)
                .WithMessage("National ID is not valid")
                .MustAsync(async (nid, cancellation) => !await _userRepository.ExistsByNationalIdAsync(nid))
                .WithMessage("National ID already exists");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .Custom((password, context) =>
                {
                    var error = PasswordValidator.Validate(password);
                    if (error != null)
                    {
                        context.AddFailure(error);
                    }
                });
        }
    }
}