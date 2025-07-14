using System.Security.Claims;
using ZTACS.Server.Services;
using ZTACS.Shared.Models;

namespace ZTACS.Server.Middleware
{
    public class RequestLoggingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
    {
        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;

            // Extract IP from X-Forwarded-For or fallback
            string ip = context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor)
                ? forwardedFor.FirstOrDefault()?.Split(',').First().Trim() ?? "127.0.0.1"
                : context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            var method = request.Method;
            var path = request.Path;
            var query = request.QueryString.HasValue ? request.QueryString.Value : string.Empty;
            var userAgent = request.Headers["User-Agent"].ToString();

            // Let the pipeline run so authentication completes first
            await next(context);

            var user = context.User?.Identity?.IsAuthenticated == true
                ? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown"
                : "Anonymous";


            using var scope = scopeFactory.CreateScope();
            var threatService = scope.ServiceProvider.GetRequiredService<IThreatDetectionService>();

            threatService.Analyze(context, new ThreatDetectionRequest
            {
                Device = userAgent,
                Endpoint = $"{method}:{path}{query}",
                UserId = user,
                Ip = ip,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}