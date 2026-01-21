using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using TrustDropFront.Common;

namespace TrustDropFront.Pages.UserProfile;

public partial class UserProfilePage : PageBase
{
    [Inject] 
    private ILocalStorageService LocalStorage { get; set; } = null!;

    [Inject] 
    private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;
    private async Task Logout()
    {
        await LocalStorage.RemoveItemAsync("jwtToken");
        var concreteAuthStateProvider = (AuthStateProvider)AuthStateProvider;
        concreteAuthStateProvider.NotifyUserLogout();
        NavigationManager.NavigateTo("/");
    }
}