namespace Domain.Models;

public class CardDetails
{
    public CardDetails(string number, int cvv, string owner, int validToMonth, int validToYear)
    {
        Number = number;
        CVV = cvv;
        Owner = owner;
        ValidToMonth = validToMonth;
        ValidToYear = validToYear;
    }

    public string Number { get; }
    public int CVV { get; }
    public string Owner { get; }
    public int ValidToMonth { get; }
    public int ValidToYear { get; }
}