using Dapper;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Data;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Infrastructure.Repositories
{
    public class PaymentLogRepository : IPaymentLogRepository
    {
        private readonly DapperContext _dapperContext;

        public PaymentLogRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task CreateAsync(PaymentLog paymentLog)
        {
            if (paymentLog == null) throw new ArgumentNullException(nameof(paymentLog));

            var sql = @"INSERT INTO PaymentLogs (RequestJson, ResponseJson)
                    VALUES (@RequestJson, @ResponseJson)";

            using var conn = _dapperContext.CreateConnection();
            await conn.ExecuteAsync(sql, paymentLog);
        }
    }
}