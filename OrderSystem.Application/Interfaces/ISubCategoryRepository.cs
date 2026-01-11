using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface ISubCategoryRepository
    {
        Task<int> CreateAsync(SubCategory subCategory);
        Task<SubCategory?> GetByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int categoryId);
        Task<IEnumerable<SubCategory>> GetAllAsync();
        Task<bool> UpdateAsync(SubCategory subCategory);
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<SubCategory>> GetByCategoryIdAsync(int id);
    }
}