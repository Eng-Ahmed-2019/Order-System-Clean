using Dapper;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Data;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Infrastructure.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly DapperContext _context;

        public SessionRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(UserSession session)
        {
            if (session == null)
                throw new ArgumentNullException("Invalid argument, Please try again");
            var sql = @"
                INSERT INTO UserSessions (Id, UserId, ExpiresAt)
                VALUES (@Id, @UserId, @ExpiresAt)";

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, session);
        }

        public async Task<UserSession?> GetByIdAsync(Guid sessionId)
        {
            var sql = @"
                SELECT *
                FROM UserSessions
                WHERE Id = @Id";

            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<UserSession>
                (sql,
                new { Id = sessionId }
                );
        }

        public async Task DeleteAsync(Guid sessionId)
        {
            var sql = "DELETE FROM UserSessions WHERE Id = @Id";

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new { Id = sessionId });
        }
    }
}