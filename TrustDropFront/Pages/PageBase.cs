using Microsoft.AspNetCore.Components;

namespace TrustDropFront.Pages;

public abstract class PageBase : ComponentBase
{
    [Inject]
    protected HttpClient httpClient { get; set; } = null!;

    protected string ErrorMessage { get; set; } = string.Empty;
    protected string SuccessMessage { get; set; } = string.Empty;
    protected bool IsLoading { get; set; } = false;
}