using MediatR;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Commands;

using OrderSystem.Application.Exceptions;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginUserHandler
        (
            IUserRepository userRepository,
            ISessionRepository sessionRepository,
            IJwtTokenGenerator jwtTokenGenerator
        )
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponseDto>Handle(LoginUserCommand command,CancellationToken cancellationToken)
        {
            Serilog.Log.Information("Login attempt for Email {Email}", command.Email);
            var user = await _userRepository.GetByEmailAsync(command.Email);
            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(command.password, user.PasswordHash))
            {
                Serilog.Log.Warning("Failed login attempt for Email {Email}", command.Email);
                throw new UnauthorizedException();
            }
            var session = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddMinutes(120)
            };
            await _sessionRepository.CreateAsync(session);
            Serilog.Log.Information(
                "Session created for UserId {UserId} with SessionId {SessionId}",
                user.Id,
                session.Id
            );
            var token = _jwtTokenGenerator.GenerateToken(
                user.Id,
                session.Id,
                session.ExpiresAt
            );
            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = session.ExpiresAt
            };
        }
    }
}