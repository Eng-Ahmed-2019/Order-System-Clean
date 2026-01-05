using MediatR;
using System.Text.Json;
using OrderSystem.Domain.Enums;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Exceptions;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, bool>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IPaymentLogRepository _logRepo;
        private readonly IOrderRepository _orderRepo;

        public ProcessPaymentHandler(
        IHttpClientFactory httpClientFactory,
        IPaymentRepository paymentRepo,
        IPaymentLogRepository logRepo,
        IOrderRepository orderRepo)
        {
            _httpClientFactory = httpClientFactory;
            _paymentRepo = paymentRepo;
            _logRepo = logRepo;
            _orderRepo = orderRepo;
        }

        public async Task<bool> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            Serilog.Log.Information(
                $"Starting payment process for OrderId {request.orderId} with amount {request.amount}",
                request.orderId,
                request.amount
            );
            var o = await _orderRepo.GetByIdAsync(request.orderId);
            if (o == null)
                throw new NotFoundException($"Not Found Any Order Match With: \"{request.orderId}\"");
            if (o.Status == "Paid")
                throw new PaidException($"Order {request.orderId} was paid before");
            var client = _httpClientFactory.CreateClient("ExternalApi");
            var paymentRequest = new
            {
                orderId = request.orderId,
                amount = request.amount
            };
            var requestJson = JsonSerializer.Serialize(paymentRequest);
            try
            {
                var response = await client.PostAsync(
                        "posts",
                        new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json")
                );
                var responseJson = await response.Content.ReadAsStringAsync();
                await _logRepo.CreateAsync(new PaymentLog
                {
                    RequestJson = requestJson,
                    ResponseJson = responseJson
                });
                await _paymentRepo.CreateAsync(new Payment
                {
                    OrderId = request.orderId,
                    Provider = "JSONPlaceholder",
                    Status = "Success",
                    TransactionId = Guid.NewGuid().ToString()
                });
                await _orderRepo.UpdateStatusAsync(request.orderId, OrderStatus.Paid);
                Serilog.Log.Information(
                    $"Payment processed successfully for OrderId {request.orderId}",
                    request.orderId
                );
                return true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Payment processing failed for OrderId {OrderId}", request.orderId);
                await _logRepo.CreateAsync(new PaymentLog
                {
                    RequestJson = requestJson,
                    ResponseJson = ex.Message
                });
                return false;
            }
        }
    }
}