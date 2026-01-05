using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> CreateAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
        Task<bool> ExistsByOrderNumberAsync(string orderNumber);
        Task UpdateStatusAsync(int orderId, string status);
    }
}