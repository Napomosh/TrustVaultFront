using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace TrustDropFront.Common;

public class RequestHandler(ApplicationSettings settings) : DelegatingHandler
{
    private readonly ApplicationSettings settings = settings;
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        if (!string.IsNullOrEmpty(settings.JwtToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.JwtToken);
        
        return base.SendAsync(request, cancellationToken);
    }
}