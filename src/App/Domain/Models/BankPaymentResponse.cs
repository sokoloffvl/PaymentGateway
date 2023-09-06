using Domain.Enums;

namespace Domain.Models;

public class BankPaymentResponse
{
    public string PaymentId { get; init; }
    public PaymentStatus Status { get; init; }
    public DeclineReason? DeclineReason { get; init; }
}