using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface ICartRepository
    {
        Task<Order?> GetUserCartAsync(int userId);
        Task<int> CreateCartAsync(Order order);
        Task AddItemAsync(OrderItem item);
        Task<bool> ItemExistsAsync(int orderId, int productId);
        Task UpdateItemQuantityAsync(int orderId, int productId, int quantity);
        Task UpdateOrderTotalAsync(int orderId);
    }
}