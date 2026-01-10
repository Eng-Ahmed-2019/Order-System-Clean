using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<int> CreateAsync(Product product);
        Task<Product?> GetByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int subCategoryId);
        Task DecreaseStockAsync(int productId, int quantity);
        Task SetInactiveIfOutOfStockAsync(int productId);
        Task<int> GetStockAsync(int productId);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
    }
}