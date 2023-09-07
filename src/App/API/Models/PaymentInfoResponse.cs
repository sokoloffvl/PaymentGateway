namespace API.Models;

public class PaymentInfoResponse
{
    public string PaymentId { get; init; }
    public string CardNumber { get; init;}
    public decimal Amount { get; init;}
    public string Currency { get; init;}
    public string Status { get; init;}
    public string DeclineReason { get; init; }
}