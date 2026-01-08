namespace OrderSystem.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(int userId, Guid sessionId, DateTime expiresAt);
    }
}