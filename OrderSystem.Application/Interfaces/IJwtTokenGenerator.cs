namespace OrderSystem.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid sessionId, DateTime ExpiresAt);
    }
}