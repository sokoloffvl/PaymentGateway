using Domain.Enums;

namespace Domain.Models;

public class Payment
{
    public Payment(string paymentId, int merchantId, CardDetails cardDetails, DateTime createdAtUtc, DateTime processedAtUtc)
    {
        PaymentId = paymentId;
        MerchantId = merchantId;
        CardDetails = cardDetails;
        CreatedAtUtc = createdAtUtc;
        ProcessedAtUtc = processedAtUtc;
        Status = PaymentStatus.Created;
    }

    public string PaymentId { get; init; }
    public int MerchantId { get; init; }
    public CardDetails CardDetails { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; }
    public PaymentStatus Status { get; private set; }
    public DeclineReason? DeclineReason { get; private set; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime ProcessedAtUtc { get; init; }

    public bool IsProcessed()
    {
        return Status != PaymentStatus.Created;
    }

    public void Processed(PaymentStatus status)
    {
        Status = status;
        DeclineReason = null;
    }
    
    public void Created()
    {
        Status = PaymentStatus.Created;
        DeclineReason = null;
    }
    
    public void Declined(DeclineReason declineReason)
    {
        Status = PaymentStatus.Declined;
        DeclineReason = declineReason;
    }
}