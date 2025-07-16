using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using ZTACS.API.Data;
using ZTACS.API.MIddleWare;
using ZTACS.API.Services;

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
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();

            var app = builder.Build();

            // 🔧 Dev tools
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
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