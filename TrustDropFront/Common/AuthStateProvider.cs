using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace TrustDropFront.Common;

public class AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage) : AuthenticationStateProvider
{
    private readonly HttpClient httpClient = httpClient;
    private readonly ILocalStorageService localStorage = localStorage;
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await localStorage.GetItemAsync<string>("jwtToken");
        // token = token?.Trim('"');
        
        if (string.IsNullOrWhiteSpace(token))
            return anonymous;
            
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        
        return new AuthenticationState(
            new ClaimsPrincipal(
                new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
    }

    public void NotifyUserAuthentication(string token)
    {
        var authenticatedUser = new ClaimsPrincipal(
            new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
        
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(anonymous);
        NotifyAuthenticationStateChanged(authState);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs == null) 
            return claims;
        keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

        if (roles != null)
        {
            if (roles is JsonElement { ValueKind: JsonValueKind.Array } jsonRoles)
            {
                var parsedRoles = jsonRoles.Deserialize<string[]>();
                    
                if (parsedRoles != null)
                    claims.AddRange(parsedRoles.Select
                        (parsedRole => new Claim(ClaimTypes.Role, parsedRole)));
            }
            else if (roles is JsonElement { ValueKind: JsonValueKind.String })
            {
                claims.Add(new Claim(ClaimTypes.Role, roles.ToString() ?? string.Empty));
            }

            keyValuePairs.Remove(ClaimTypes.Role);
        }

        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)));

        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: 
                base64 += "=="; 
                break;
            case 3: 
                base64 += "=";
                break;
        }
        return Convert.FromBase64String(base64);
    }
}