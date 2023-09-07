namespace API.Authentication;

public static class MerchantApiKeysProvider
{
    public static readonly Dictionary<string, int> ApiKeys = new() { {"one", 1}, {"two", 2}, {"three", 3}};
    public static int? GetMerchantId(string apiKey)
    {
        if (ApiKeys.TryGetValue(apiKey, out var merchantId))
        {
            return merchantId;
        }

        return null;
    }
}