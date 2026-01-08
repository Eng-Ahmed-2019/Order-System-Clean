using System.Net;
using System.Text.Json;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.API.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public CustomExceptionMiddleware(
            RequestDelegate next,
            ILogger<CustomExceptionMiddleware> logger,
            IServiceScopeFactory factory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = factory;
        }

        public async Task Invoke(HttpContext context)
        {
            var traceId = context.TraceIdentifier;
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, traceId);
                using var scope = _scopeFactory.CreateScope();
                var logRepository = scope.ServiceProvider.GetRequiredService<ILogRepository>();
                if (logRepository is not null)
                {
                    try
                    {
                        await logRepository.CreatedException(ex, traceId);
                    }
                    catch { }
                }
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            string traceId)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            object response;

            switch (exception)
            {
                case ValidationException ve:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = ve.Errors,
                        TraceId = traceId
                    };
                    break;
                case UnauthorizedException ue:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    response = new
                    {
                        Success = false,
                        Message = ue.Message,
                        TraceId = traceId
                    };
                    break;
                case ConfigurationException configuration:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    response = new
                    {
                        Success = false,
                        Message = configuration.Message,
                        TraceId = traceId
                    };
                    break;
                case NotFoundException nfe:
                    statusCode = (int)HttpStatusCode.NotFound;
                    response = new
                    {
                        Success = false,
                        Message = nfe.Message,
                        TraceId = traceId
                    };
                    break;
                case PaidException pa:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        Success = false,
                        Message = pa.Message,
                        TraceId = traceId
                    };
                    break;
                case BusinessException business:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        Success = false,
                        Message = business.Message,
                        TraceId = traceId
                    };
                    break;
                case CartException cart:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        Success = false,
                        Message = cart.Message,
                        TraceId = traceId
                    };
                    break;
                case PaymentGatewayException paymentGateway:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        Success = false,
                        Message = paymentGateway.Message,
                        TraceId = traceId
                    };
                    break;
                case IsActiveException isactive:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        Success = false,
                        Message = isactive.Message,
                        TraceId = traceId
                    };
                    break;
                case NotReadyException notready:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        Success = false,
                        Message = notready.Message,
                        TraceId = traceId
                    };
                    break;
                case OrderException order:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        Success = false,
                        Message = order.Message,
                        TraceId = traceId
                    };
                    break;
                case StockException stock:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        Success = false,
                        Message = stock.Message,
                        TraceId = traceId
                    };
                    break;
                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;

                    _logger.LogError(
                        exception,
                        "Unhandled exception occurred. TraceId: {TraceId}",
                        traceId
                    );

                    response = new
                    {
                        Success = false,
                        Message = "Unexpected error occurred. Please try again later.",
                        TraceId = traceId
                    };
                    break;
            }
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}