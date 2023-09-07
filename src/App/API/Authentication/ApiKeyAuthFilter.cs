using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Authentication;

public static class AuthConstants
{
    public const string ApiKeyHeader = "x-api-key";
}

public class ApiKeyAuthFilter : IAuthorizationFilter
{
    private static readonly Dictionary<string, int> ApiKeys = new() { {"one", 1}, {"two", 2}, {"three", 3}};
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeader, out var apiKey))
        {
            context.Result = new UnauthorizedObjectResult("Missing api key");
            return;
        }
        
        if (!ApiKeys.TryGetValue(apiKey, out var merchantId))
        {
            context.Result = new UnauthorizedObjectResult("Unknown api key");
            return;
        }
        

        var principal = new MerchantPrincipal(new GenericIdentity($"Merchant {merchantId}", "ApiKeyAuth"), null)
        {
            MerchantId = merchantId
        };
        context.HttpContext.User = principal;
    }
}
    
public class MerchantPrincipal : GenericPrincipal
{
    public MerchantPrincipal(IIdentity identity, string[]? roles)
        : base(identity, roles)
    {
    }
    public int MerchantId {get; set;}
}