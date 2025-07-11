using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection; // Add this line
using Microsoft.Extensions.Http;
using MudBlazor;
using MudBlazor.Services;
using System.Net.Http;
using ZTACS.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// 🔐 Auth0 + JWT  
builder.Services.AddHttpClient("ZTACS.ServerAPI", client =>
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Scoped HttpClient for general use  
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ZTACS.ServerAPI"));
builder.Services.AddMudServices();
// Add OIDC Authentication  
builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = "https://auth.blackhatbadshah.com/realms/ztacs";
    options.ProviderOptions.ClientId = "25pSHCWTZQHPupUPAXimVmvcIDKaUIEK";
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.RedirectUri = "https://localhost:7017/welcome";

    // Optional but recommended
    options.ProviderOptions.PostLogoutRedirectUri = "https://localhost:7017/bye";
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("offline_access");
});


await builder.Build().RunAsync();