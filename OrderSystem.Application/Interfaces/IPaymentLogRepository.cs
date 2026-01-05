using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces
{
    public interface IPaymentLogRepository
    {
        Task CreateAsync(PaymentLog log);
    }
}