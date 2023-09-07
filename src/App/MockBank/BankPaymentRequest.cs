namespace MockBank;

public class BankPaymentRequest
{
    public string PaymentId { get; init; } = string.Empty;
    public string CardNumber { get; init;} = string.Empty;
    public string CVV { get; init;} = string.Empty;
    public string CardOwner { get; init;} = string.Empty;
    public decimal Amount { get; init;}
    public string Currency { get; init;} = string.Empty;
    public int ValidToYear { get; init;}
    public int ValidToMonth { get; init;}
}