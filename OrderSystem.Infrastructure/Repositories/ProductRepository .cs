using Dapper;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Data;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DapperContext _context;

        public ProductRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(Product product)
        {
            var sql = @"INSERT INTO Products
                        (SubCategoryId, Name, Description, Price, Stock, IsActive)
                        VALUES
                        (@SubCategoryId, @Name, @Description, @Price, @Stock, @IsActive);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

            using var conn = _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, product);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Products WHERE Id = @Id";
            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
        }

        public async Task<bool> ExistsByNameAsync(string name, int subCategoryId)
        {
            var sql = @"SELECT COUNT(1)
                        FROM Products
                        WHERE Name = @Name AND SubCategoryId = @SubCategoryId";

            using var conn = _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(
                sql,
                new { Name = name, SubCategoryId = subCategoryId }
            ) > 0;
        }

        public async Task DecreaseStockAsync(int productId, int quantity)
        {
            var sql = @"UPDATE Products
                SET Stock = Stock - @Quantity
                WHERE Id = @ProductId";

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new
            {
                ProductId = productId,
                Quantity = quantity
            });
        }

        public async Task SetInactiveIfOutOfStockAsync(int productId)
        {
            var sql = @"UPDATE Products
                SET IsActive = 0
                WHERE Id = @ProductId AND Stock <= 0";

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new { ProductId = productId });
        }

        public async Task<int> GetStockAsync(int productId)
        {
            var sql = @"SELECT Stock
                FROM Products
                WHERE Id = @ProductId AND IsActive = 1";

            using var conn = _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, new { ProductId = productId });
        }
    }
}