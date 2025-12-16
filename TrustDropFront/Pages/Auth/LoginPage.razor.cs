using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using TrustDropFront.Common;
using TrustDropFront.Models.Auth;

namespace TrustDropFront.Pages.Auth;

public partial class LoginPage : PageBase
{
    private LoginModel Model { get; set; } = new();

    [Inject]
    private ILocalStorageService localStorage { get; set; }
    
    [Inject]
    private AuthenticationStateProvider authStateProvider { get; set; }
    
    [Inject]
    private NavigationManager navigationManager { get; set; }

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
                
                var jsonResponse = await Network.ParseJsonAsync(response);
                var jwtToken = jsonResponse.GetProperty("jwtToken").ToString();
                
                await localStorage.SetItemAsync("jwtToken", jwtToken);
                var concreteAuthStateProvider = (AuthStateProvider)authStateProvider;
                concreteAuthStateProvider.NotifyUserAuthentication(jwtToken);
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