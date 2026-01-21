using Dapper;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Data;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Infrastructure.Repositories
{
    public class LoginLockoutRepository : ILoginLockoutRepository
    {
        private readonly DapperContext _context;

        public LoginLockoutRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<LoginLockout?> GetAsync(string key)
        {
            var sql = """
                SELECT *
                FROM LoginLockouts
                WHERE KeyValue = @Key
            """;

            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<LoginLockout>(sql, new { Key = key });
        }

        public async Task RegisterFailureAsync(string key)
        {
            var sql = """
                MERGE LoginLockouts AS target
                USING (SELECT @Key AS KeyValue) AS source
                ON target.KeyValue = source.KeyValue
                WHEN MATCHED THEN
                    UPDATE SET
                        FailedAttempts = FailedAttempts + 1,
                        LockedUntil = CASE
                            WHEN FailedAttempts + 1 >= 5
                            THEN DATEADD(MINUTE, 15, GETUTCDATE())
                            ELSE LockedUntil
                        END
                WHEN NOT MATCHED THEN
                    INSERT (KeyValue, FailedAttempts)
                    VALUES (@Key, 1);
            """;

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new { Key = key });
        }

        public async Task ResetAsync(string key)
        {
            var sql = """
                DELETE FROM LoginLockouts
                WHERE KeyValue = @Key
            """;

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new { Key = key });
        }
    }
}