using API.Models;
using Domain.Models;

namespace API.Translators;

public static class CreatePaymentRequestTranslator
{
    public static Payment ToDomain(this CreatePaymentRequest request)
    {
        var cardDetails = new CardDetails(request.CardNumber, request.CVV, request.CardOwner, request.ValidToMonth, request.ValidToYear);
        return new Payment(request.PaymentId, 0, cardDetails, request.Currency, request.Amount);
    }
}