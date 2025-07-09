using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http;
using Microsoft.Extensions.Http;
using ZTACS.Client;
using Microsoft.Extensions.DependencyInjection; // Add this line

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// 🔐 Auth0 + JWT  
builder.Services.AddHttpClient("ZTACS.ServerAPI", client =>
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Scoped HttpClient for general use  
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ZTACS.ServerAPI"));

// Add OIDC Authentication  
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.Authority = "https://blackhat.auth0.com";
    options.ProviderOptions.ClientId = "25pSHCWTZQHPupUPAXimVmvcIDKaUIEK"; // 👈 Replace with your actual Client ID  
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
    options.ProviderOptions.DefaultScopes.Add("offline_access");

    options.ProviderOptions.PostLogoutRedirectUri = "/";
    options.ProviderOptions.RedirectUri = "/";
});

await builder.Build().RunAsync();