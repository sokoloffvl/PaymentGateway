using Domain.Models;

namespace Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetPaymentAsync(string paymentId);
    Task CreatePaymentAsync(string paymentId);
    Task UpdatePaymentAsync(Payment payment);
}