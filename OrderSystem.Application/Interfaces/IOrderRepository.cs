using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id, int userId);
        Task UpdateStatusAsync(int orderId, string status);
        Task<IEnumerable<Order>> GetByStatusAsync(string status);
    }
}