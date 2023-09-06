using System.Collections.Concurrent;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Domain.Translators;
using Polly;

namespace Domain.Services;

public class PaymentProcessingService : IPaymentProcessingService
{
    private readonly IBankClient bankClient;
    private readonly IPaymentRepository paymentRepository;

    public PaymentProcessingService(IBankClient bankClient,
        IPaymentRepository paymentRepository)
    {
        this.bankClient = bankClient;
        this.paymentRepository = paymentRepository;
    }

    public async Task Process(string paymentId)
    {
        var payment = await paymentRepository.GetPaymentAsync(paymentId);
        if (payment is null || payment.IsProcessed())
            return;

        var bankRequest = payment.ToRequest();
        var response = await TryProcessPayment(bankRequest);

        if (response.DeclineReason is not null)
            payment.Declined(response.DeclineReason.Value);
        else
            payment.Processed(response.Status);
        
        await paymentRepository.GetPaymentAsync(paymentId);
    }

    private async Task<BankPaymentResponse> TryProcessPayment(BankPaymentRequest request)
    {
        var policy = Policy
            .HandleResult<BankPaymentResponse>(r => r.Status == PaymentStatus.ProcessingFailed)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        var response = await policy.ExecuteAsync(async x => await bankClient.ProcessPayment(request), null);

        return response;
    }

}