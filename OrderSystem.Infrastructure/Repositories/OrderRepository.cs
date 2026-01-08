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

        public async Task<Order?> GetByIdAsync(int id)
        {
            if (id == 0) throw new ArgumentNullException(nameof(id));

            var sql = "SELECT * FROM Orders WHERE Id = @Id";

            using var connection = _dapperContext.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Order>(sql, new { Id = id });
        }

        public async Task<bool> ExistsByOrderNumberAsync(int id)
        {
            var sql = "SELECT COUNT(1) FROM Orders WHERE Id = @Id";

            using var conn = _dapperContext.CreateConnection();
            var count = await conn.ExecuteScalarAsync<int>(sql, new { Id = id });

            return count > 0;
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