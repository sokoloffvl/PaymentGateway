namespace Domain.Models;

public class CardDetails
{
    public CardDetails(string number, int cvv, string owner)
    {
        Number = number;
        CVV = cvv;
        Owner = owner;
    }

    public string Number { get; init; }
    public int CVV { get; init; }
    public string Owner { get; init; }
    public int ValidToMonth { get; init; }
    public int ValidToYear { get; init; }
}