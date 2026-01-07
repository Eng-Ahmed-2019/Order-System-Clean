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
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IProductRepository _productRepo;

        public ProcessPaymentHandler(
        IHttpClientFactory httpClientFactory,
        IPaymentRepository paymentRepo,
        IPaymentLogRepository logRepo,
        IOrderRepository orderRepo,
        IOrderItemRepository orderItemRepository,
        IProductRepository productRepository)
        {
            _httpClientFactory = httpClientFactory;
            _paymentRepo = paymentRepo;
            _logRepo = logRepo;
            _orderRepo = orderRepo;
            _orderItemRepository = orderItemRepository;
            _productRepo = productRepository;
        }

        public async Task<bool> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            Serilog.Log.Information(
                $"Starting payment process for OrderId {request.orderId}",
                request.orderId
            );
            var o = await _orderRepo.GetByIdAsync(request.orderId);
            if (o == null)
                throw new NotFoundException($"Not Found Any Order Match With: \"{request.orderId}\"");
            if (o.Status != OrderStatus.PaymentPending)
                throw new Exception(
                    $"Order {request.orderId} is not ready for payment"
                );
            var items = await _orderItemRepository.GetByOrderIdAsync(request.orderId);
            foreach (var item in items)
            {
                var stock = await _productRepo.GetStockAsync(item.ProductId);

                if (stock < item.Quantity)
                {
                    throw new Exception(
                        $"Insufficient stock for product {item.ProductId}"
                    );
                }
            }
            var client = _httpClientFactory.CreateClient("ExternalApi");
            if (!items.Any())
                throw new Exception("Order has no items");
            var totalAmount = items.Sum(x => x.UnitPrice * x.Quantity);
            var paymentRequest = new
            {
                orderId = request.orderId,
                amount = totalAmount
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
                var paymentStatus = response.IsSuccessStatusCode
                    ? PaymentStatus.Success
                    : PaymentStatus.Failed;
                var orderStatus = paymentStatus == PaymentStatus.Success
                    ? OrderStatus.Paid
                    : OrderStatus.Failed;
                await _paymentRepo.CreateAsync(new Payment
                {
                    OrderId = request.orderId,
                    Provider = "JSONPlaceholder",
                    Status = paymentStatus,
                    TransactionId = Guid.NewGuid().ToString()
                });
                await _orderRepo.UpdateStatusAsync(request.orderId, orderStatus);
                Serilog.Log.Information(
                    $"Payment processed status for OrderId {request.orderId}",
                    request.orderId
                );
                if (paymentStatus == PaymentStatus.Success)
                {
                    foreach (var item in items)
                    {
                        await _productRepo.DecreaseStockAsync(item.ProductId, item.Quantity);
                        await _productRepo.SetInactiveIfOutOfStockAsync(item.ProductId);
                    }
                }
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