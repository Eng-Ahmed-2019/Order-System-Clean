using System.Text.Json;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.API.Middlewares
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public SessionValidationMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context,ISessionRepository repository)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var sid = context.User.FindFirst("sid")?.Value;
                if (sid == null)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        Message = "Session not found. Please log in."
                    };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }
                var session = await repository.GetByIdAsync(Guid.Parse(sid));
                if (session == null || session.ExpiresAt < DateTime.UtcNow)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        Message = "Session expired or invalid. Please log in again."
                    };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }
            }
            await _requestDelegate(context);
        }
    }
}