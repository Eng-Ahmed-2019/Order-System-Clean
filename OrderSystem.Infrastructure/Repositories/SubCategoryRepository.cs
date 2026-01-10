using Dapper;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Data;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Infrastructure.Repositories
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        private readonly DapperContext _context;

        public SubCategoryRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(SubCategory subCategory)
        {
            var sql = @"INSERT INTO SubCategories (CategoryId, Name, Description)
                        VALUES (@CategoryId, @Name, @Description);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

            using var conn = _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, subCategory);
        }

        public async Task<SubCategory?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM SubCategories WHERE Id = @Id";
            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<SubCategory>(sql, new { Id = id });
        }

        public async Task<bool> ExistsByNameAsync(string name, int categoryId)
        {
            var sql = @"SELECT COUNT(1)
                        FROM SubCategories
                        WHERE Name = @Name AND CategoryId = @CategoryId";

            using var conn = _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, new { Name = name, CategoryId = categoryId }) > 0;
        }

        public async Task<IEnumerable<SubCategory>> GetAllAsync()
        {
            var sql = @"SELECT * FROM SubCategories ORDER BY Id DESC";
            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<SubCategory>(sql);
        }

        public async Task<bool> UpdateAsync(SubCategory subCategory)
        {
            var sql = @"UPDATE SubCategories
                SET Name = @Name,
                    Description = @Description,
                    CategoryId = @CategoryId
                WHERE Id = @Id";

            using var conn = _context.CreateConnection();
            return await conn.ExecuteAsync(sql, subCategory) > 0;
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            var sql = "SELECT COUNT(1) FROM SubCategories WHERE Name = @Name";
            using var conn = _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, new { Name = name }) > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM SubCategories WHERE Id = @Id";
            using var conn = _context.CreateConnection();
            return await conn.ExecuteAsync(sql, new { Id = id }) > 0;
        }
    }
}