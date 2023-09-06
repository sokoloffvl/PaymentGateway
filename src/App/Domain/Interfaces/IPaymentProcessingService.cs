using Domain.Models;

namespace Domain.Interfaces;

public interface IPaymentProcessingService
{
    public Task Process(string paymentId);
}