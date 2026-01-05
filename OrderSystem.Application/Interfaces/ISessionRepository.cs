using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface ISessionRepository
    {
        Task CreateAsync(UserSession session);
        Task<UserSession?> GetByIdAsync(Guid sessionId);
        Task DeleteAsync(Guid sessionId);
    }
}