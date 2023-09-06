namespace Domain.Models;

public class BankPaymentRequest
{
    public string PaymentId { get; init; }
    public string CardNumber { get; init;}
    public int CVV { get; init;}
    public string CardOwner { get; init;}
    public decimal Amount { get; init;}
    public string Currency { get; init;}
    public int ValidToYear { get; init;}
    public int ValidToMonth { get; init;}
}