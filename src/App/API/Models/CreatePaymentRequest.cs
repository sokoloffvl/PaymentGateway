using System.ComponentModel.DataAnnotations;
using API.Validation;

namespace API.Models;

public class CreatePaymentRequest
{
    [Required]
    public string PaymentId { get; init; }

    [CreditCard]
    public string CardNumber { get; init;}

    [CVVCodeValidator]
    public string CVV { get; init;}

    [Required]
    public string CardOwner { get; init;}

    [Range(0, 10000000)]
    public decimal Amount { get; init;}

    [CurrencyValidator("GPB", "USD", "EUR")]
    public string Currency { get; init;}

    [Range(23, 99)]
    public int ValidToYear { get; init;}

    [Range(1, 12)]
    public int ValidToMonth { get; init;}
    public int MerchantId { get; set; }
}