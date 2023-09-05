namespace MockBank;

public class BankPaymentResponse
{
    public string PaymentId { get; init; }
    public BankPaymentStatus Status { get; init; }
    public DeclineReason? DeclineReason { get; init; }
}