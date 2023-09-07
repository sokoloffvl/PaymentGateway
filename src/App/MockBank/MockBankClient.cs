namespace MockBank;

public interface IMockBankClient
{
    public Task<BankPaymentResponse> ProcessPayment(BankPaymentRequest paymentRequest);
}

public class MockBankClient : IMockBankClient
{
    private readonly Random random;
    
    public MockBankClient()
    {
        random = new Random();
    }

    //Real-life bank client would likely be async and I guess we want to imitate that
    public async Task<BankPaymentResponse> ProcessPayment(BankPaymentRequest paymentRequest)
    {
        var statuses = Enum.GetValues(typeof(BankPaymentStatus));
        var randomStatus = (BankPaymentStatus) (statuses.GetValue(random.Next(statuses.Length)) ?? BankPaymentStatus.Declined);
        DeclineReason? randomDeclineReason = null;
        if (randomStatus == BankPaymentStatus.Declined)
        {
            var declineReasons = Enum.GetValues(typeof(DeclineReason));
            randomDeclineReason = (DeclineReason) (declineReasons.GetValue(random.Next(declineReasons.Length)) ?? DeclineReason.InvalidCardDetails);
        }

        var response = new BankPaymentResponse
        {
            PaymentId = paymentRequest.PaymentId,
            Status = randomStatus,
            DeclineReason = randomDeclineReason
        };
        //Some random delay 0-0.5 seconds
        await Task.Delay(random.Next(500));

        return response;
    }
}