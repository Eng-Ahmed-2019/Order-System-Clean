using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<int> CreateAsync(Category category);
        Task<Category?> GetByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<bool> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
    }
}