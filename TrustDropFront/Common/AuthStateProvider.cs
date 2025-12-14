using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace TrustDropFront.Common;

public class AuthStateProvider(ProtectedSessionStorage storage) : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage storage = storage;
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var tokenResult = await storage.GetAsync<string>("authToken");

        if (!tokenResult.Success || string.IsNullOrEmpty(tokenResult.Value))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        
        return new AuthenticationState(new ClaimsPrincipal(
                    new ClaimsIdentity(ParseClaimsFromJwt(tokenResult.Value), "jwt")));
    }
    
    public void NotifyAuthStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var json = System.Text.Encoding.UTF8.GetString
                                    (Convert.FromBase64String(AddPadding(jwt.Split('.')[1])));
        var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        return dict?.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? "")) ?? [];
    }

    private static string AddPadding(string base64)
    {
        return base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
    }
}