using System.Net.Http.Json;
using TrustDropFront.Common;
using TrustDropFront.Models.Auth;

namespace TrustDropFront.Pages.Auth;

public partial class RegisterPage : PageBase
{
    private RegisterModel Model { get; set; } = new();

    private bool RegistrationSuccessful { get; set; } = false;

    private async Task DoRegister()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        IsLoading = false;
        RegistrationSuccessful = false;

        try
        {
            var response = await ApiHttpClient.PostAsJsonAsync(RequestConstants.REQUEST_AUTH_REGISTER, Model);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Registration successful!";
                Model = new RegisterModel();
                RegistrationSuccessful = true;
            }
            else
            {
                ErrorMessage = $"Registration failed: { await Network.ParseServerErrorAsync(response) }";
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