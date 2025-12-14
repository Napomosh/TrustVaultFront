using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using TrustDropFront.Common;
using TrustDropFront.Models.Auth;

namespace TrustDropFront.Components.Pages.Auth;

public partial class LoginPage : PageBase
{
    private LoginModel Model { get; set; } = new();

    [Inject] 
    private ProtectedSessionStorage Storage { get; set; } = null!;

    [Inject] 
    private AuthStateProvider StateProvider { get; set; } = null!;

    private async Task DoLogin()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        IsLoading = true;

        try
        {
            var response = await httpClient.PostAsJsonAsync(RequestConstants.REQUEST_LOGIN, Model);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Login successful!";
                Model = new LoginModel();
                
                await Storage.SetAsync("authToken", await Network.ParseServerErrorAsync(response));
                StateProvider.NotifyAuthStateChanged();
            }
            else
            {
                ErrorMessage = $"Login failed: { await Network.ParseServerErrorAsync(response) }";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = Network.DefaultErrorMessage(ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }
}