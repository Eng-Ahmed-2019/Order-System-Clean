using MediatR;
using System.Text.Json;
using System.Net.Http.Headers;
using OrderSystem.Domain.Enums;
using OrderSystem.Domain.Entities;
using Microsoft.Extensions.Options;
using OrderSystem.Application.Settings;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class ProcessStripePaymentHandler
         : IRequestHandler<ProcessStripePaymentCommand, bool>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPaymentLogRepository _logRepository;
        private readonly IPaymentRepository _paymentRepo;
        private readonly StripeSettings _stripeSettings;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogRepository _repository;

        public ProcessStripePaymentHandler(
            IHttpClientFactory httpClientFactory,
            IPaymentLogRepository logRepository,
            IPaymentRepository paymentRepo,
            IOptions<StripeSettings> stripeOptions,
            IOrderRepository orderRepository,
            ILogRepository repository
            )
        {
            _httpClientFactory = httpClientFactory;
            _logRepository = logRepository;
            _paymentRepo = paymentRepo;
            _stripeSettings = stripeOptions.Value;
            _orderRepository = orderRepository;
            _repository = repository;
        }

        public async Task<bool> Handle(ProcessStripePaymentCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_stripeSettings.SecretKey))
            {
                Serilog.Log.Error(
                    "Stripe SecretKey is missing. OrderId {OrderId}",
                    command.orderId
                );
                return false;
            }
            if (!_stripeSettings.SecretKey.StartsWith("sk_test_") &&
            !_stripeSettings.SecretKey.StartsWith("sk_live_"))
            {
                var msg = $"Invalid Stripe SecretKey format. OrderId {command.orderId}";
                Serilog.Log.Error(msg);
                await _repository.CreatedException(new Exception(msg), command.orderId.ToString());
                return false;
            }
            Serilog.Log.Information(
                "Starting Stripe payment for OrderId {OrderId}",
                command.orderId
            );
            if (command == null) return false;
            var o = await _orderRepository.GetByIdAsync(command.orderId);
            if (o == null)
                throw new NotFoundException($"Not Found Any Order Match With: \"{command.orderId}\"");
            if (o.Status == "Paid")
                throw new PaidException($"Order {command.orderId} was paid before");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://api.stripe.com/v1/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _stripeSettings.SecretKey
                );
            var paymentData = new Dictionary<string, string>
            {
                { "amount", ((int)(command.amount * 100)).ToString() },
                {"currency","usd" },
                {"payment_method_types[]","card" }
            };
            var content = new FormUrlEncodedContent(paymentData);
            try
            {
                var response = await client.PostAsync(
                "payment_intents",
                content,
                cancellationToken
                );
                var responseJson = await response.Content.ReadAsStringAsync();
                await _logRepository.CreateAsync(
                    new PaymentLog
                    {
                        RequestJson = JsonSerializer.Serialize(paymentData),
                        ResponseJson = responseJson
                    }
                );
                await _paymentRepo.CreateAsync(
                    new Payment
                    {
                        OrderId = command.orderId,
                        Provider = "Stripe",
                        Status = response.IsSuccessStatusCode
                        ? PaymentStatus.Success
                        : PaymentStatus.Failed,
                        TransactionId = Guid.NewGuid().ToString()
                    }
                );
                await _orderRepository.UpdateStatusAsync(command.orderId, OrderStatus.Paid);
                Serilog.Log.Information(
                    "Stripe payment finished for OrderId {OrderId} with Status {Status}",
                    command.orderId,
                    response.IsSuccessStatusCode
                );
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex,
                    "Stripe payment failed for OrderId {OrderId}",
                    command.orderId
                );
                await _logRepository.CreateAsync(
                    new PaymentLog
                    {
                        RequestJson = JsonSerializer.Serialize(paymentData),
                        ResponseJson = ex.Message
                    }
                );
                return false;
            }
        }
    }
}