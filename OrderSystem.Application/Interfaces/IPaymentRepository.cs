using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<int> CreateAsync(Payment payment);
        Task UpdateStatusAsync(int paymentId, string status);
    }
}