using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TrustDropFront;
using TrustDropFront.Common;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<RequestHandler>();
builder.Services.AddHttpClient("TrustDropAPI", client => 
    {
        client.BaseAddress = new Uri("http://localhost:5101/api/");
    })
    .AddHttpMessageHandler<RequestHandler>();
builder.Services.AddScoped(sp => 
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("TrustDropAPI"));

builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

builder.Services.AddSingleton<ApplicationSettings>();

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();