using Domain.Interfaces;

namespace Domain.Services;

public class MaskingService : IMaskingService
{
    public string MaskCardNumber(string cardNumber)
    {
        var firstFourChars = new string(cardNumber.Take(4).ToArray());
        var lastFourChars = new string(cardNumber.TakeLast(4).ToArray());
        return $"{firstFourChars} *** {lastFourChars}";
    }
}