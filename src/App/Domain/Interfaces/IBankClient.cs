using Domain.Models;

namespace Domain.Interfaces;

public interface IBankClient
{
    public Task<BankPaymentResponse> ProcessPayment(BankPaymentRequest paymentRequest);
}