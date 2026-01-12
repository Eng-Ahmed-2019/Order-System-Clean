using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.API.Middlewares
{
    public class UnifiedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UnifiedMiddleware> _logger;

        public UnifiedMiddleware(RequestDelegate next, ILogger<UnifiedMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context,
            ISessionRepository sessionRepository,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var sid = context.User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

                if (sid == null || userId == null) throw new UnauthorizedException("Invalid token");

                var session = await sessionRepository.GetByIdAsync(Guid.Parse(sid));
                if (session == null || session.ExpiresAt < DateTime.UtcNow)
                    throw new UnauthorizedException("Session expired. Please log in again.");

                var exp = context.User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
                if (exp != null)
                {
                    var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime;
                    if (expDate.Subtract(DateTime.UtcNow).TotalMinutes <= 5)
                    {
                        var newToken = jwtTokenGenerator.GenerateToken(
                            int.Parse(userId),
                            session.Id,
                            session.ExpiresAt,
                            role!
                        );
                        context.Response.Headers["X-Refreshed-Token"] = newToken;
                    }
                }
            }
            await _next(context);
        }
    }
}