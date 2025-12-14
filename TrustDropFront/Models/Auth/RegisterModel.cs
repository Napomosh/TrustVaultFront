using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrustDropFront.Models.Auth;

public class RegisterModel
{
    [Required(ErrorMessage = "Nickname is required")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Nickname must be between 3 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Nickname can only contain letters and numbers")]
    [Display(Name = "Nickname")]
    [JsonPropertyName("login")]
    public string Nickname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [StringLength(255, ErrorMessage = "Email must be short than 255 characters")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email address")]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [JsonIgnore]
    public string RepeatPassword { get; set; } = string.Empty;
}
