using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface ISubCategoryRepository
    {
        Task<int> CreateAsync(SubCategory subCategory);
        Task<SubCategory?> GetByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int categoryId);
    }
}