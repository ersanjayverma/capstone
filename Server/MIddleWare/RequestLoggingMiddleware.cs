using System.Security.Claims;
using ZTACS.Server.Services;
using ZTACS.Shared.Models;

namespace ZTACS.Server.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public RequestLoggingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;

            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var method = request.Method;
            var path = request.Path;
            var query = request.QueryString.HasValue ? request.QueryString.Value : string.Empty;
            var userAgent = request.Headers["User-Agent"].ToString();

            var user = context.User?.Identity?.IsAuthenticated == true
                ? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? context.User.FindFirst(ClaimTypes.Email)?.Value
                  ?? context.User.FindFirst(ClaimTypes.Name)?.Value
                  ?? "Unknown"
                : "Anonymous";

            // Scoped service
            using var scope = _scopeFactory.CreateScope();
            var threatService = scope.ServiceProvider.GetRequiredService<IThreatDetectionService>();

            // Await async call
            threatService.Analyze(new ThreatDetectionRequest
            {
                Device = userAgent,
                Endpoint = $"{method}:{path}{query}",
                UserId = user,
                Ip = ip,
                Timestamp = DateTime.UtcNow
            });

            await _next(context);
        }
    }
}