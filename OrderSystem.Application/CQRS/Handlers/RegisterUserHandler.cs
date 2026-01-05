using MediatR;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public RegisterUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(RegisterUserCommand command,CancellationToken cancellation)
        {
            Serilog.Log.Information(
                "Registering new user with Email {Email}",
                command.Email
            );
            if (command == null) return;
            var user = new User
            {
                FullName = command.FullName,
                Email = command.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                NationalId = command.NationaId
            };
            await _userRepository.CreateAsync(user);
            Serilog.Log.Information(
                "User registered successfully with Email {Email}",
                command.Email
            );
        }
    }
}