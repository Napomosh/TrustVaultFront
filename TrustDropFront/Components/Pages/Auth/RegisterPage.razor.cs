using TrustDropFront.Common;
using TrustDropFront.Models.Auth;

namespace TrustDropFront.Components.Pages.Auth;

public partial class RegisterPage : PageBase
{
    private RegisterModel Model { get; set; } = new();

    private async Task DoRegister()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        IsLoading = false;

        try
        {
            var response = await httpClient.PostAsJsonAsync(RequestConstants.REQUEST_REGISTER, Model);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Registration successful!";
                Model = new RegisterModel();
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