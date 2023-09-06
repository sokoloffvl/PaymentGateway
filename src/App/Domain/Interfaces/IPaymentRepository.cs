using Domain.Models;

namespace Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetPaymentAsync(string paymentId);
    Task CreatePaymentAsync(Payment payment);
    Task UpdatePaymentAsync(Payment payment);
}