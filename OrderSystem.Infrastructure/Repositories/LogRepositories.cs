using Dapper;
using OrderSystem.Infrastructure.Data;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.Exceptions;

namespace OrderSystem.Infrastructure.Repositories
{
    public class LogRepositories : ILogRepository
    {
        private readonly DapperContext _dapperContext;

        public LogRepositories(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task CreatedException(Exception exception, string traceId)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            var level = exception switch
            {
                ValidationException => "Warning",
                NotFoundException => "Info",
                UnauthorizedException => "Warning",
                _ => "Error"
            };

            var sql = @"
                INSERT INTO Logs (Level, Message, Exception, TraceId, CreatedAt)
                VALUES (@Level, @Message, @Exception, @TraceId, @CreatedAt);
            ";

            var parameters = new
            {
                Level = level,
                Message = exception.Message,
                Exception = exception.ToString(),
                TraceId = traceId,
                CreatedAt = DateTime.UtcNow
            };

            using var conn = _dapperContext.CreateConnection();
            await conn.ExecuteAsync(sql, parameters);
        }
    }
}