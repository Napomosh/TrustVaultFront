using System.Text.Json;

namespace TrustDropFront.Common;

public static class Network
{
    public static async Task<string> ParseServerErrorAsync(HttpResponseMessage source)
    {
        var content = await source.Content.ReadAsStringAsync();
        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;
            return root.GetProperty("error").GetString() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public static async Task<JsonElement> ParseJsonAsync(HttpResponseMessage source)
    {
        var content = await source.Content.ReadAsStringAsync();
        
        try
        {
            return JsonDocument.Parse(content).RootElement.GetProperty("value");
        }
        catch
        {
            return new JsonElement();
        }
    }
    
    public static string DefaultErrorMessage(string serverMessage) => $"An error occurred on the server: {serverMessage}";
}