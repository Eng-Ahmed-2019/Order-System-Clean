/*
using MediatR;
using System.Text;
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
                throw new NotReadyException($"Order {request.orderId} is not ready for payment");
            var items = await _orderItemRepository.GetByOrderIdAsync(request.orderId);
            foreach (var item in items)
            {
                var stock = await _productRepo.GetStockAsync(item.ProductId);

                if (stock < item.Quantity)
                {
                    throw new StockException($"Insufficient stock for product {item.ProductId}");
                }
            }
            var client = _httpClientFactory.CreateClient("ExternalApi");
            if (!items.Any())
                throw new OrderException("Order has no items");
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
                    new StringContent(requestJson, Encoding.UTF8, "application/json"),
                    cancellationToken
                );
                var responseJson = await response.Content.ReadAsStringAsync();
                await _logRepo.CreateAsync(new PaymentLog
                {
                    RequestJson = requestJson,
                    ResponseJson = responseJson
                });
                var success = response.IsSuccessStatusCode;
                
                //var paymentStatus = response.IsSuccessStatusCode
                    //? PaymentStatus.Success
                    //: PaymentStatus.Failed;
                //var orderStatus = paymentStatus == PaymentStatus.Success
                    //? OrderStatus.Paid
                    //: OrderStatus.Failed;
                
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.Enums;

await _paymentRepo.CreateAsync(new Payment
{
    OrderId = request.orderId,
    Provider = "JSONPlaceholder",
    Status = success ? PaymentStatus.Success.ToString() : PaymentStatus.Failed.ToString(),
    TransactionId = Guid.NewGuid().ToString()
});
await _orderRepo.UpdateStatusAsync(
    request.orderId,
    success ? OrderStatus.Paid : OrderStatus.Failed
);
Serilog.Log.Information(
    $"Payment processed status for OrderId {request.orderId}",
    request.orderId
);
if (success)
{
    foreach (var item in items)
    {
        await _productRepo.DecreaseStockAsync(item.ProductId, item.Quantity);
        await _productRepo.SetInactiveIfOutOfStockAsync(item.ProductId);
    }
}
return success;
            }
            catch (HttpRequestException ex)
            {
                Serilog.Log.Error(ex, "Payment Gateway error");
return false;
            }
            catch (TaskCanceledException ex)
            {
                Serilog.Log.Error(ex, "Payment Gateway timeout");
return false;
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
 */
using MediatR;
using System.Text;
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
        private readonly IOrderItemRepository _orderItemRepo;
        private readonly IProductRepository _productRepo;

        public ProcessPaymentHandler(
            IHttpClientFactory httpClientFactory,
            IPaymentRepository paymentRepo,
            IPaymentLogRepository logRepo,
            IOrderRepository orderRepo,
            IOrderItemRepository orderItemRepo,
            IProductRepository productRepo)
        {
            _httpClientFactory = httpClientFactory;
            _paymentRepo = paymentRepo;
            _logRepo = logRepo;
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _productRepo = productRepo;
        }

        public async Task<bool> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            Serilog.Log.Information(
                "Starting payment process for OrderId {OrderId}",
                request.orderId
            );

            var order = await _orderRepo.GetByIdAsync(request.orderId);
            if (order == null)
                throw new NotFoundException($"Order not found: {request.orderId}");

            if (order.Status != OrderStatus.PaymentPending)
                throw new NotReadyException($"Order {request.orderId} is not ready for payment");

            var items = (await _orderItemRepo.GetByOrderIdAsync(request.orderId)).ToList();
            if (!items.Any())
                throw new OrderException("Order has no items");

            foreach (var item in items)
            {
                var stock = await _productRepo.GetStockAsync(item.ProductId);
                if (stock < item.Quantity)
                    throw new StockException($"Insufficient stock for product {item.ProductId}");
            }

            var totalAmount = items.Sum(i => i.UnitPrice * i.Quantity);
            var paymentRequest = new
            {
                orderId = request.orderId,
                amount = totalAmount
            };

            var requestJson = JsonSerializer.Serialize(paymentRequest);

            try
            {
                var client = _httpClientFactory.CreateClient("ExternalApi");

                var response = await client.PostAsync(
                    "posts",
                    new StringContent(requestJson, Encoding.UTF8, "application/json"),
                    cancellationToken
                );

                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

                await _logRepo.CreateAsync(new PaymentLog
                {
                    RequestJson = requestJson,
                    ResponseJson = responseJson
                });

                if (!response.IsSuccessStatusCode)
                    throw new PaymentGatewayException(
                        $"Payment gateway failed with status {response.StatusCode}"
                    );

                await _paymentRepo.CreateAsync(new Payment
                {
                    OrderId = request.orderId,
                    Provider = "JSONPlaceholder",
                    Status = PaymentStatus.Success.ToString(),
                    TransactionId = Guid.NewGuid().ToString()
                });

                await _orderRepo.UpdateStatusAsync(order.Id, OrderStatus.Paid);

                foreach (var item in items)
                {
                    await _productRepo.DecreaseStockAsync(item.ProductId, item.Quantity);
                    await _productRepo.SetInactiveIfOutOfStockAsync(item.ProductId);
                }

                Serilog.Log.Information(
                    "Payment completed successfully for OrderId {OrderId}",
                    request.orderId
                );

                return true;
            }
            catch (HttpRequestException ex)
            {
                Serilog.Log.Error(ex, "Payment gateway connection error");

                throw new PaymentGatewayException(
                    "Payment service is currently unavailable"
                );
            }
            catch (TaskCanceledException ex)
            {
                Serilog.Log.Error(ex, "Payment gateway timeout");

                throw new PaymentGatewayException(
                    "Payment service timeout"
                );
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(
                    ex,
                    "Payment processing failed for OrderId {OrderId}",
                    request.orderId
                );

                await _logRepo.CreateAsync(new PaymentLog
                {
                    RequestJson = requestJson,
                    ResponseJson = ex.Message
                });
                throw;
            }
        }
    }
}