using Dapper;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Data;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DapperContext _dapperContext;

        public CategoryRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<int>CreateAsync(Category category)
        {
            var sql = @"INSERT INTO Categories (Name, Description)
                        VALUES (@Name, @Description);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

            using var conn = _dapperContext.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, category);
        }

        public async Task<Category?>GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Categories WHERE Id = @Id";
            using var conn = _dapperContext.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id });
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            var sql = "SELECT COUNT(1) FROM Categories WHERE Name = @Name";
            using var conn = _dapperContext.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, new { Name = name }) > 0;
        }
    }
}