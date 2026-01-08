using System.Text;
using System.Text.Json;
using OrderSystem.Domain.Enums;
using OrderSystem.Domain.Entities;
using Microsoft.Extensions.Hosting;
using OrderSystem.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
namespace OrderSystem.Infrastructure.BackgroundJobs
{
    public class PaymentRetryBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PaymentRetryBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await RetryPendingPayments(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task RetryPendingPayments(CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();

            var orderRepo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var orderItemRepo = scope.ServiceProvider.GetRequiredService<IOrderItemRepository>();
            var productRepo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var paymentRepo = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
            var logRepo = scope.ServiceProvider.GetRequiredService<IPaymentLogRepository>();
            var httpFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

            var pendingOrders = await orderRepo.GetByStatusAsync(OrderStatus.PaymentPending);

            if (!pendingOrders.Any())
                return;

            var client = httpFactory.CreateClient("ExternalApi");

            foreach (var order in pendingOrders)
            {
                var items = (await orderItemRepo.GetByOrderIdAsync(order.Id)).ToList();
                if (!items.Any()) continue;

                bool hasStock = true;
                foreach (var item in items)
                {
                    var stock = await productRepo.GetStockAsync(item.ProductId);
                    if (stock < item.Quantity)
                    {
                        hasStock = false;
                        break;
                    }
                }

                if (!hasStock) continue;

                var totalAmount = items.Sum(i => i.UnitPrice * i.Quantity);

                var requestJson = JsonSerializer.Serialize(new
                {
                    orderId = order.Id,
                    amount = totalAmount
                });

                try
                {
                    var response = await client.PostAsync(
                        "posts",
                        new StringContent(requestJson, Encoding.UTF8, "application/json"),
                        token
                    );

                    var responseJson = await response.Content.ReadAsStringAsync(token);

                    await logRepo.CreateAsync(new PaymentLog
                    {
                        RequestJson = requestJson,
                        ResponseJson = responseJson
                    });

                    if (!response.IsSuccessStatusCode)
                        continue;

                    await paymentRepo.CreateAsync(new Payment
                    {
                        OrderId = order.Id,
                        Provider = "RetryService",
                        Status = PaymentStatus.Success,
                        TransactionId = Guid.NewGuid().ToString()
                    });

                    await orderRepo.UpdateStatusAsync(order.Id, OrderStatus.Paid);

                    foreach (var item in items)
                    {
                        await productRepo.DecreaseStockAsync(item.ProductId, item.Quantity);
                        await productRepo.SetInactiveIfOutOfStockAsync(item.ProductId);
                    }
                }
                catch (Exception ex)
                {
                    await logRepo.CreateAsync(new PaymentLog
                    {
                        RequestJson = requestJson,
                        ResponseJson = ex.Message
                    });
                }
            }
        }
    }
}