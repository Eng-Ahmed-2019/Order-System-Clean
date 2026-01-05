using OrderSystem.Application.Interfaces;
using OrderSystem.Application.Exceptions;

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

        public async Task InvokeAsync(HttpContext context, ISessionRepository sessionRepository)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var sid = context.User.FindFirst("sid")?.Value;
                if (sid == null)
                {
                    throw new UnauthorizedException("Session not found. Please log in again.");
                }
                var session = await sessionRepository.GetByIdAsync(Guid.Parse(sid));
                if (session == null || session.ExpiresAt < DateTime.UtcNow)
                {
                    throw new UnauthorizedException("Session expired. Please log in again.");
                }
            }
            await _next(context);
        }
    }
}