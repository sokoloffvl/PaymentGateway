using Domain.Interfaces;
using Infrastructure.Translators;
using MockBank;
using BankPaymentRequest = Domain.Models.BankPaymentRequest;
using BankPaymentResponse = Domain.Models.BankPaymentResponse;

namespace Infrastructure.BankCommunication;

public class BankClient : IBankClient
{
    private readonly IMockBankClient mockBankClient;

    public BankClient(IMockBankClient mockBankClient)
    {
        this.mockBankClient = mockBankClient;
    }

    public async Task<BankPaymentResponse> ProcessPayment(BankPaymentRequest paymentRequest)
    {
        var request = paymentRequest.ToMockRequest();
        var response = await mockBankClient.ProcessPayment(request);
        return response.ToDomain();
    }
}