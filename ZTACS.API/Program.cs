using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using ZTACS.API.Data;
using ZTACS.API.MIddleWare;
using ZTACS.API.Services;
using Microsoft.OpenApi.Models;

namespace ZTACS.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var _config = builder.Configuration;
            builder.Services.AddScoped<IThreatDetectionService, ThreatDetectionService>();

            builder.Services.AddHttpClient();
            builder.Services.AddDbContext<ThreatDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));
            // 🔐 Add JWT Bearer authentication
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = _config["Auth0:Domain"];
                    options.RequireHttpsMetadata = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = _config["Auth0:Domain"],
                        ValidateAudience = true,
                        ValidAudience = _config["Auth0:Audience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });

            // 🔧 Add services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ZTACS API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter a valid JWT token",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new[] { "Bearer" } }
    });
});
            builder.Services.AddHttpClient();

            var app = builder.Build();
app.UsePathBase("/api/v1");
            // 🔧 Dev tools
            app.Use((context, next) =>
{
    context.Request.PathBase = "/api/v1";
    return next();
});


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/api/v1/swagger/v1/swagger.json", "ZTACS API V1");
                    c.RoutePrefix = "swagger"; // So it's accessible via /api/v1/swagger/index.html
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // 👈 Add this BEFORE Authorization
            app.UseAuthorization();

            app.UseMiddleware<RequestLoggingMiddleware>();
            app.MapControllers();

            app.Run();
        }
    }
}