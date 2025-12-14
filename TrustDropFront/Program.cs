using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using TrustDropFront.Common;
using TrustDropFront.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthStateProvider>();

builder.Services.AddHttpClient("TrustDropAPI", client =>
{
    var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
    client.BaseAddress = new Uri(apiBaseUrl!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();