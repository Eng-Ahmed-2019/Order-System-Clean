using Dapper;
using OrderSystem.Application.Interfaces;
using OrderSystem.Infrastructure.Data;

namespace OrderSystem.Infrastructure.Repositories
{
    public class BanIpRepository : IBanIpRepository
    {
        private readonly DapperContext _context;

        public BanIpRepository(DapperContext context)
        {
            _context = context;
        }

        
        public async Task<bool> IsBanned(string ip)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM BannedIps
                WHERE IpAddress = @Ip
                  AND BanExpiresAt > GETUTCDATE()";

            using var conn = _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, new { Ip = ip }) > 0;
        }

        public async Task BanAsync(string ip, TimeSpan duration, string reason)
        {
            var sql = """
                INSERT INTO BannedIps (IpAddress, BannedUntil, Reason)
                VALUES (@Ip, DATEADD(SECOND, @Seconds, GETUTCDATE()), @Reason)
            """;
            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new
            {
                Ip = ip,
                Reason = reason,
                DurationSeconds = (int)duration.TotalSeconds
            });
        }
    }
}