namespace MockBank;

public class BankPaymentResponse
{
    public string PaymentId { get; init; } = string.Empty;
    public BankPaymentStatus Status { get; init; }
    public DeclineReason? DeclineReason { get; init; }
}