using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<int> CreateAsync(User user);
        Task<User> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByNationalIdAsync(string nationalId);
    }
}