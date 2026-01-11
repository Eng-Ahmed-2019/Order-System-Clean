using Dapper;
using OrderSystem.Domain.Enums;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Data;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly DapperContext _context;

        public CartRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetUserCartAsync(int userId)
        {
            var sql = @"SELECT TOP 1 *
                        FROM Orders
                        WHERE UserId = @UserId AND Status = @Status";

            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Order>(sql, new { UserId = userId, Status = OrderStatus.Cart });
        }

        public async Task<int> CreateCartAsync(Order order)
        {
            var sql = @"INSERT INTO Orders (UserId, Status, TotalAmount)
                        VALUES (@UserId, @Status, 0);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

            using var conn = _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, order);
        }

        public async Task<bool> ItemExistsAsync(int orderId, int productId)
        {
            var sql = @"SELECT COUNT(1)
                        FROM OrderItems
                        WHERE OrderId = @OrderId AND ProductId = @ProductId";

            using var conn = _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(
                sql,
                new { OrderId = orderId, ProductId = productId }
            ) > 0;
        }

        public async Task AddItemAsync(OrderItem item)
        {
            var sql = @"INSERT INTO OrderItems
                        (OrderId, ProductId, Quantity, UnitPrice)
                        VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)";

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, item);
        }

        public async Task UpdateItemQuantityAsync(
            int orderId,
            int productId,
            int quantity)
        {
            var sql = @"UPDATE OrderItems
                        SET Quantity = Quantity + @Quantity
                        WHERE OrderId = @OrderId AND ProductId = @ProductId";

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity
            });
        }

        public async Task UpdateOrderTotalAsync(int orderId)
        {
            var sql = @"UPDATE Orders
                        SET TotalAmount = ISNULL((
                            SELECT SUM(Quantity * UnitPrice)
                            FROM OrderItems
                            WHERE OrderId = @OrderId
                        ),0)
                        WHERE Id = @OrderId";

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new { OrderId = orderId });
        }

        public async Task<IEnumerable<OrderItem>> GetCartItemsAsync(int orderId)
        {
            var sql = @"SELECT oi.*, p.Name, p.Description
                FROM OrderItems oi
                INNER JOIN Products p ON p.Id = oi.ProductId
                WHERE oi.OrderId = @OrderId";

            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<OrderItem>(sql, new { OrderId = orderId });
        }

        public async Task<bool> RemoveItemAsync(int id)
        {
            var sql = @"DELETE FROM OrderItems
                WHERE Id = @Id";

            using var conn = _context.CreateConnection();
            return await conn.ExecuteAsync(sql, new { Id = id }) > 0;
        }

        public async Task DeleteCartAsync(int orderId)
        {
            var sql = "DELETE FROM Orders WHERE Id = @Id";
            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new { Id = orderId });
        }
    }
}