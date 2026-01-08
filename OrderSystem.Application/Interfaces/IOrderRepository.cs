using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<bool> ExistsByOrderNumberAsync(int id);
        Task UpdateStatusAsync(int orderId, string status);
        Task<IEnumerable<Order>> GetByStatusAsync(string status);
    }
}