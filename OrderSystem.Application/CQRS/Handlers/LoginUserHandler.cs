using MediatR;
using Microsoft.AspNetCore.Http;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Queries;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class LoginUserHandler
    : IRequestHandler<LoginUserCommand, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _http;

        public LoginUserHandler(
            IUserRepository userRepository,
            ISessionRepository sessionRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IMediator mediator,
            IHttpContextAccessor http)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mediator = mediator;
            _http = http;
        }

        public async Task<LoginResponseDto> Handle(
            LoginUserCommand command,
            CancellationToken cancellationToken)
        {
            var ip = _http.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var key = $"{ip}:{command.Email}";

            // Check lockout
            var lockout = await _mediator.Send(new GetLoginLockoutQuery(key));
            if (lockout?.LockedUntil > DateTime.UtcNow)
                throw new BusinessException("Account temporarily locked");

            // Validate credentials
            var user = await _userRepository.GetByEmailAsync(command.Email);
            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(command.password, user.PasswordHash))
            {
                await _mediator.Send(new RegisterLoginFailureCommand(key));
                throw new UnauthorizedException("Invalid email or password");
            }

            // Success → Reset lockout
            await _mediator.Send(new ResetLoginLockoutCommand(key));

            // 4️⃣ Create session
            var session = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddMinutes(120)
            };

            await _sessionRepository.CreateAsync(session);

            var token = _jwtTokenGenerator.GenerateToken(
                user.Id,
                session.Id,
                session.ExpiresAt,
                user.Role
            );

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = session.ExpiresAt
            };
        }
    }
}