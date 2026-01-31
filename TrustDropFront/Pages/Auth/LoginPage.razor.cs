using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using TrustDropFront.Common;
using TrustDropFront.Models.Auth;

namespace TrustDropFront.Pages.Auth;

public partial class LoginPage : PageBase
{
    private LoginModel Model { get; set; } = new();

    [Inject]
    private AuthenticationStateProvider AuthStateProviderProperty { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject] 
    private ApplicationSettings ApplicationSettings { get; set; } = null!;

    private async Task DoLogin()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        IsLoading = true;

        try
        {
            var response = await ApiHttpClient.PostAsJsonAsync(RequestConstants.REQUEST_AUTH_LOGIN, Model);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Login successful!";
                Model = new LoginModel();
                
                var jsonResponse = await Network.ParseJsonAsync(response);

                ApplicationSettings.JwtToken = jsonResponse.GetProperty("token").ToString();
                
                if (AuthStateProviderProperty is AuthStateProvider concreteAuthStateProvider)
                    concreteAuthStateProvider.NotifyUserAuthentication(ApplicationSettings.JwtToken);
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