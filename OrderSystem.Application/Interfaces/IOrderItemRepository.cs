using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId);
    }
}