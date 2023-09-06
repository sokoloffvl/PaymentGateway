using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repository;

public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly IMemoryCache cache;

    public InMemoryPaymentRepository(IMemoryCache cache)
    {
        this.cache = cache;
    }
    public async Task<Payment?> GetPaymentAsync(string paymentId)
    {
        var exists = cache.TryGetValue<Payment>(paymentId, out var payment);
        return exists ? payment : null;
    }

    public async Task CreatePaymentAsync(Payment payment)
    {
        if (cache.TryGetValue<Payment>(payment.PaymentId, out _))
            throw new InvalidOperationException($"Duplicate key {payment.PaymentId} found");

        cache.Set(payment.PaymentId, payment);
    }

    public async Task UpdatePaymentAsync(Payment payment)
    {
        if (!cache.TryGetValue<Payment>(payment.PaymentId, out _))
            throw new ArgumentException($"Payment {payment.PaymentId} not found");

        cache.Set(payment.PaymentId, payment);
    }
}