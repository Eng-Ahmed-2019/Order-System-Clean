using Dapper;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Data;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DapperContext _dapperContext;

        public OrderRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<Order?> GetByIdAsync(int id, int userId)
        {
            var sql = @"SELECT *
                FROM Orders
                WHERE Id = @Id AND UserId = @UserId";

            using var conn = _dapperContext.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Order>(
                sql,
                new { Id = id, UserId = userId }
            );
        }

        public async Task UpdateStatusAsync(int orderId, string status)
        {
            var sql = "UPDATE Orders SET Status = @Status WHERE Id = @Id";

            using var conn = _dapperContext.CreateConnection();
            await conn.ExecuteAsync(sql, new { Id = orderId, Status = status });
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
        {
            var sql = "SELECT * FROM Orders WHERE Status = @Status";

            using var conn = _dapperContext.CreateConnection();
            return await conn.QueryAsync<Order>(sql, new { Status = status });
        }
    }
}