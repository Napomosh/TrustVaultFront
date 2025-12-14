using Microsoft.AspNetCore.Components;

namespace TrustDropFront.Components.Pages;

public abstract class PageBase : ComponentBase
{
    [Inject] private IHttpClientFactory factory { get; set; } = null!;

    protected string ErrorMessage { get; set; } = string.Empty;
    protected string SuccessMessage { get; set; } = string.Empty;
    protected bool IsLoading { get; set; } = false;
    
    protected HttpClient httpClient => factory.CreateClient("TrustDropAPI");
}