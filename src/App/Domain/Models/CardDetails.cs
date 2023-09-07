namespace Domain.Models;

public class CardDetails
{
    public CardDetails(string number, string cvv, string owner, int validToMonth, int validToYear)
    {
        Number = number;
        CVV = cvv;
        Owner = owner;
        ValidToMonth = validToMonth;
        ValidToYear = validToYear;
    }

    public string Number { get; }
    public string CVV { get; }
    public string Owner { get; }
    public int ValidToMonth { get; }
    public int ValidToYear { get; }
}