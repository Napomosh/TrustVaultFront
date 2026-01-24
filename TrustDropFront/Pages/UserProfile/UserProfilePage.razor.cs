using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using TrustDropFront.Common;

namespace TrustDropFront.Pages.UserProfile;

public partial class UserProfilePage : PageBase
{
    [Inject] 
    private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;
    
    private async Task Logout()
    {
        var response = await ApiHttpClient.GetAsync(RequestConstants.REQUEST_AUTH_LOGOUT);
        
        if (response.IsSuccessStatusCode)
        {
            if (AuthStateProvider is AuthStateProvider concreteAuthStateProvider)
                concreteAuthStateProvider.NotifyUserLogout();
            NavigationManager.NavigateTo("/login");
        }
    }
}