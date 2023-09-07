using API.Models;
using Swashbuckle.AspNetCore.Filters;

namespace API.ExampleProviders;

public class CardPaymentExampleProvider : IExamplesProvider<CreatePaymentRequest>
{
    public CreatePaymentRequest GetExamples()
    {
        return new CreatePaymentRequest
        {
            PaymentId = Guid.NewGuid().ToString(),
            CardNumber = "378282246310005",
            CVV = "123",
            CardOwner = "Card Owner",
            Amount = 100,
            Currency = "USD",
            ValidToYear = 29,
            ValidToMonth = 11
        };
    }
}