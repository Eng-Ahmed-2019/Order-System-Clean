using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface ILoginLockoutRepository
    {
        Task<LoginLockout?> GetAsync(string key);
        Task RegisterFailureAsync(string key);
        Task ResetAsync(string key);
    }
}