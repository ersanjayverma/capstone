using Auth0.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using MudBlazor.Services;
using ZTACS.Components;

namespace ZTACS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var _config = builder.Configuration;

            // Add services to the container
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddControllers(); // ✅ Required for API endpoints

            builder.Services.AddAuth0WebAppAuthentication(options =>
            {
                options.Domain = _config["Auth0:Domain"];
                options.ClientId = _config["Auth0:ClientId"];
            });

            // Add Swagger + optional Auth0 integration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ZTACS API",
                    Version = "v1",
                    Description = "Zero Trust Access Control System API"
                });

                // Optional: OAuth2 support for Swagger UI
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"https://{_config["Auth0:Domain"]}/authorize"),
                            TokenUrl = new Uri($"https://{_config["Auth0:Domain"]}/oauth/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenID" },
                                { "profile", "User profile" }
                            }
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        },
                        new[] { "openid", "profile" }
                    }
                });
            });

            builder.Services.AddMudServices(); // ✅ MudBlazor support

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.OAuthClientId(_config["Auth0:ClientId"]);
                    options.OAuthUsePkce(); // Recommended for public clients
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
     
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();
            app.MapControllers(); // ✅ Maps API routes like /api/*

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            app.Run();
        }
    }
}
