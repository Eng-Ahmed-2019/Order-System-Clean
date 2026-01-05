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
                await RetryFailedPayments();
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task RetryFailedPayments()
        {
            using var scope = _scopeFactory.CreateScope();
            var paymentRepo = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
            var logRepo = scope.ServiceProvider.GetRequiredService<IPaymentLogRepository>();
            var httpFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var failedPayments = await paymentRepo.GetFailedAsync();
            var client = httpFactory.CreateClient("ExternalApi");
            foreach(var payment in failedPayments)
            {
                // This code, I will use it for simulation...!
                // And connect to https://jsonplaceholder.typicode.com/posts
                var requestJson = JsonSerializer.Serialize(new
                {
                    orderId = payment.OrderId,
                    amount = 0
                });
                try
                {
                    var response = await client.PostAsync(
                            "posts",
                            new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json")
                    );
                    var responseJson = await response.Content.ReadAsStringAsync();
                    await logRepo.CreateAsync(new PaymentLog
                    {
                        RequestJson = requestJson,
                        ResponseJson = responseJson
                    });
                    await paymentRepo.UpdateStatusAsync(payment.OrderId, PaymentStatus.Success);
                }
                catch(Exception ex)
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