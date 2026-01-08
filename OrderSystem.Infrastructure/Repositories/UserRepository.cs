using Dapper;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Data;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _dapperContext;

        public UserRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<int> CreateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var sql = @"
                INSERT INTO Users (FullName, NationalId, Email, PasswordHash, CreatedAt)
                VALUES (@FullName, @NationalId, @Email, @PasswordHash, GETUTCDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            using var conn = _dapperContext.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, user);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var sql = "SELECT * FROM Users WHERE Email = @Email";

            using var conn = _dapperContext.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<User>(sql, new { Email = email }) ??
                throw new Exception($"Not found user match with: {email}");
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
            using var conn = _dapperContext.CreateConnection();
            var count = await conn.ExecuteScalarAsync<int>(sql, new { Email = email });
            return count > 0;
        }

        public async Task<bool> ExistsByNationalIdAsync(string nationalId)
        {
            var sql = "SELECT COUNT(1) FROM Users WHERE NationalId = @NationalId";
            using var conn = _dapperContext.CreateConnection();
            var count = await conn.ExecuteScalarAsync<int>(sql, new { NationalId = nationalId });
            return count > 0;
        }
    }
}