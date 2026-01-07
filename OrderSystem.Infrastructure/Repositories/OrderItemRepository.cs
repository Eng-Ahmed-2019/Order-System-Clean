using Dapper;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Data;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Infrastructure.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly DapperContext _dapperContext;

        public OrderItemRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId)
        {
            var sql = @"SELECT *
                        FROM OrderItems
                        WHERE OrderId = @OrderId";

            using var conn = _dapperContext.CreateConnection();
            return await conn.QueryAsync<OrderItem>(sql, new { OrderId = orderId });
        }
    }
}