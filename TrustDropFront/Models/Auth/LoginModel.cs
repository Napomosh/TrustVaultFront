using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrustDropFront.Models.Auth;

public class LoginModel
{
    [Required(ErrorMessage = "Login is required")]
    [StringLength(255, ErrorMessage = "Login must be short than 255 characters")]
    [Display(Name = "Login")]
    [JsonPropertyName("login")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}