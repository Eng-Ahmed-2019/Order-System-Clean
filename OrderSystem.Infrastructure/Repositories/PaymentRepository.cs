using Dapper;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Data;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DapperContext _dapperContext;

        public PaymentRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<int> CreateAsync(Payment payment)
        {
            if (payment == null) throw new ArgumentNullException(nameof(payment));

            var sql = @"INSERT INTO Payments (OrderId, Provider, Status, TransactionId)
                    VALUES (@OrderId, @Provider, @Status, @TransactionId);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

            using var conn = _dapperContext.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, payment);
        }

        public async Task UpdateStatusAsync(int paymentId,string status)
        {
            var sql = "UPDATE Payments SET Status = @Status WHERE Id = @Id";
            using var conn = _dapperContext.CreateConnection();
            await conn.ExecuteAsync(sql, new { Id = paymentId, Status = status });
        }
    }
}