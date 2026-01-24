using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using TrustDropFront.Models.Auth;

namespace TrustDropFront.Common;

public class AuthStateProvider(HttpClient httpClient) : AuthenticationStateProvider
{
    private readonly HttpClient httpClient = httpClient;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var response = await httpClient.GetAsync(RequestConstants.REQUEST_AUTH_ME);

            if (!response.IsSuccessStatusCode)
                return AnonymousState();

            var userInfo = await response.Content.ReadFromJsonAsync<UserAuthInfo>();

            if (userInfo == null /*|| !userInfo.IsAuthenticated*/)
                return AnonymousState();

            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
                new (ClaimTypes.Name, userInfo.Name),
                new (ClaimTypes.Email, userInfo.Email)
            };

            // if (userInfo.CurrentTenantId.HasValue)
            // {
            //     claims.Add(new Claim("TenantId", userInfo.CurrentTenantId.Value.ToString()));
            // }

            var identity = new ClaimsIdentity(claims, "ServerAuth");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch (Exception)
        {
            return AnonymousState();
        }
    }

    public async Task NotifyUserAuthentication()
    {
        var authState = await GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        NotifyAuthenticationStateChanged(authState);
    }
    private AuthenticationState AnonymousState() 
        => new (new ClaimsPrincipal(new ClaimsIdentity()));
}