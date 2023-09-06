using Domain.Enums;
using MockBank;
using BankPaymentRequest = Domain.Models.BankPaymentRequest;
using BankPaymentResponse = Domain.Models.BankPaymentResponse;
using DeclineReason = Domain.Enums.DeclineReason;

namespace Infrastructure.Translators;

public static class BankRequestsTranslator
{
    public static MockBank.BankPaymentRequest ToMockRequest(this BankPaymentRequest request)
    {
        return new MockBank.BankPaymentRequest
        {
            PaymentId = request.PaymentId,
            CardNumber = request.CardNumber,
            CVV = request.CVV.ToString(),
            CardOwner = request.CardOwner,
            Amount = request.Amount,
            Currency = request.Currency,
            ValidToYear = request.ValidToYear,
            ValidToMonth = request.ValidToMonth
        };
    }
    
    public static BankPaymentResponse ToDomain(this MockBank.BankPaymentResponse response)
    {
        return new BankPaymentResponse
        {
            PaymentId = response.PaymentId,
            Status = response.Status.ToDomain(),
            DeclineReason = response.DeclineReason.ToDomain()
        };
    }

    private static PaymentStatus ToDomain(this BankPaymentStatus status)
    {
        return status switch
        {
            BankPaymentStatus.ProcessingFailed => PaymentStatus.ProcessingFailed,
            BankPaymentStatus.Succeeded => PaymentStatus.Succeeded,
            BankPaymentStatus.Declined => PaymentStatus.Declined,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown BankPaymentStatus")
        };
    }
    
    private static DeclineReason? ToDomain(this MockBank.DeclineReason? reason)
    {
        return reason switch
        {
            MockBank.DeclineReason.InsufficientFunds => DeclineReason.InsufficientFunds,
            MockBank.DeclineReason.InvalidCardDetails => DeclineReason.InvalidCardDetails,
            MockBank.DeclineReason.FraudDetected => DeclineReason.FraudDetected,
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, "Unknown DeclineReason")
        };
    }
}