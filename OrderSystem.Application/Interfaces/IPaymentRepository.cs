using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<int> CreateAsync(Payment payment);
        Task<IEnumerable<Payment>> GetFailedAsync();
        Task UpdateStatusAsync(int paymentId, string status);
    }
}