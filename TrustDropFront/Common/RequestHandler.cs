using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace TrustDropFront.Common;

public class RequestHandler(ApplicationSettings settings, AuthenticationStateProvider authStateProvider) : DelegatingHandler
{
    private readonly ApplicationSettings _settings = settings;
    private readonly AuthenticationStateProvider _authStateProvider = authStateProvider;
    private static readonly SemaphoreSlim RefreshSemaphore = new(1, 1);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var isRefreshRequest = request.RequestUri?.AbsolutePath.EndsWith(RequestConstants.REQUEST_AUTH_REFRESH, StringComparison.OrdinalIgnoreCase) == true;

        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        
        if (!string.IsNullOrEmpty(_settings.JwtToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.JwtToken);
        
        var response = await base.SendAsync(request, cancellationToken);

        if (isRefreshRequest || response.StatusCode != HttpStatusCode.Unauthorized) 
            return response;
        
        await RefreshSemaphore.WaitAsync(cancellationToken);
        try
        {
            // Check if another thread already refreshed the token
            if (request.Headers.Authorization?.Parameter == _settings.JwtToken)
            {
                // Token wasn't changed by another thread, so we actually need to refresh
                var success = await RefreshTokenAsync(request, cancellationToken);
                if (!success)
                    return response; // Return original 401 response if refresh fails
            }

            // Retry with new token
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.JwtToken);
            response.Dispose();
            response = await base.SendAsync(request, cancellationToken);
        }
        finally
        {
            RefreshSemaphore.Release();
        }

        return response;
    }

    private async Task<bool> RefreshTokenAsync(HttpRequestMessage originalRequest, CancellationToken cancellationToken)
    {
        if (originalRequest.RequestUri == null) 
            return false;

        var baseUrl = originalRequest.RequestUri.GetLeftPart(UriPartial.Authority);
        var refreshUrl = $"{baseUrl}/api/{RequestConstants.REQUEST_AUTH_REFRESH}";
        
        using var refreshRequest = new HttpRequestMessage(HttpMethod.Post, refreshUrl);
        refreshRequest.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        var refreshResponse = await base.SendAsync(refreshRequest, cancellationToken);

        if (refreshResponse.IsSuccessStatusCode)
        {
            var jsonResponse = await Network.ParseJsonAsync(refreshResponse);
            if (jsonResponse.ValueKind != JsonValueKind.Undefined && jsonResponse.TryGetProperty("token", out var tokenProp))
            {
                var newToken = tokenProp.GetString();
                if (!string.IsNullOrEmpty(newToken))
                {
                    _settings.JwtToken = newToken;
                    
                    if (_authStateProvider is AuthStateProvider concreteProvider)
                        concreteProvider.NotifyUserAuthentication(_settings.JwtToken);
                    
                    return true;
                }
            }
        }

        // Refresh failed, clear state
        _settings.JwtToken = null;
        if (_authStateProvider is AuthStateProvider concreteProviderLogout)
            concreteProviderLogout.NotifyUserLogout();

        return false;
    }
}