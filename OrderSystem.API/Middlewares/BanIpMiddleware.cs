using OrderSystem.Application.Interfaces;

namespace OrderSystem.API.Middlewares
{
    public class BanIpMiddleware
    {
        private readonly RequestDelegate _next;

        public BanIpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IBanIpRepository repo)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();
            if (ip != null && await repo.IsBanned(ip))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("IP banned.");
                return;
            }
            await _next(context);
        }
    }
}