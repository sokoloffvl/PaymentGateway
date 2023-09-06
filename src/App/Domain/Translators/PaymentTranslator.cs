using Domain.Models;

namespace Domain.Translators;

public static class PaymentTranslator
{
    public static BankPaymentRequest ToRequest(this Payment payment)
    {
        return new BankPaymentRequest
        {
            PaymentId = payment.PaymentId,
            CardNumber = payment.CardDetails.Number,
            CVV = payment.CardDetails.CVV,
            CardOwner = payment.CardDetails.Owner,
            Amount = payment.Amount,
            Currency = payment.Currency,
            ValidToYear = payment.CardDetails.ValidToYear,
            ValidToMonth = payment.CardDetails.ValidToMonth
        };
    }
}