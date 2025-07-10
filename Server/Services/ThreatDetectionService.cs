using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZTACS.Server.Data;
using ZTACS.Shared.Entities;
using ZTACS.Shared.Models;

namespace ZTACS.Server.Services
{

    public class ThreatDetectionService : IThreatDetectionService
    {
        private readonly ThreatDbContext _db;
        private readonly HashSet<string> _blacklistedIps;

        public ThreatDetectionService(ThreatDbContext db)
        {
            _db = db;
            // Load blacklisted IPs from DB into a HashSet for fast lookup
            _blacklistedIps = _db.BlacklistedIps
                .AsNoTracking()
                .Select(b => b.Ip)
                .ToHashSet();
        }

        public ThreatDetectionResponse Analyze(ThreatDetectionRequest request)
        {
            var riskScore = 0;
            var reasons = new List<string>();

            // Log current time
            var nowUtc = DateTime.UtcNow;

            // Rule 0: Anonymous user
            if (string.IsNullOrWhiteSpace(request.UserId) || request.UserId == "anonymous")
            {
                riskScore += 25;
                reasons.Add("Anonymous user");
            }

            // Rule 1: Static IP blacklist
            if (_blacklistedIps.Contains(request.Ip))
            {
                riskScore += 80;
                reasons.Add("Blacklisted IP");
            }

            // Rule 2: Sensitive endpoint
            if (request.Endpoint.Contains("/admin", StringComparison.OrdinalIgnoreCase))
            {
                riskScore += 20;
                reasons.Add("Accessed admin endpoint");
            }

            // Rule 3: Unusual hours
            var hour = request.Timestamp.ToUniversalTime().Hour;
            if (hour < 6 || hour > 22)
            {
                riskScore += 10;
                reasons.Add("Unusual access hour");
            }

            // Rule 4: Analyze login history
            var lastLogins = _db.LoginEvents
                .Where(e => e.UserId == request.UserId)
                .OrderByDescending(e => e.Timestamp)
                .Take(20)
                .ToList();

            if (!lastLogins.Any())
            {
                riskScore += 15;
                reasons.Add("First login detected");
            }
            else
            {
                var recentIps = lastLogins.Select(e => e.Ip).Distinct().ToList();
                if (!recentIps.Contains(request.Ip))
                {
                    riskScore += 20;
                    reasons.Add("New IP address");
                }

                var recentDevices = lastLogins.Select(e => e.Device).Distinct().ToList();
                if (!recentDevices.Contains(request.Device))
                {
                    riskScore += 15;
                    reasons.Add("New device used");
                }

                // Rule 5: Brute force (5+ logins in 5 mins)
                var rapidLogins = lastLogins.Count(e => e.Timestamp > request.Timestamp.AddMinutes(-5));
                if (rapidLogins > 5)
                {
                    riskScore += 25;
                    reasons.Add("High-frequency login attempts");
                }

                // Rule 6: Repeat request in < 10 seconds
                var veryRecent = lastLogins.FirstOrDefault(e =>
                    e.Endpoint == request.Endpoint &&
                    e.Ip == request.Ip &&
                    e.Device == request.Device &&
                    (request.Timestamp - e.Timestamp).TotalSeconds < 10);

                if (veryRecent != null)
                {
                    riskScore += 20;
                    reasons.Add("Repeated request too soon");
                }
            }

            _db.LoginEvents.Add(new LoginEvent()
            {
                UserId = request.UserId,
                Ip = request.Ip,
                Device = request.Device,
                Endpoint = request.Endpoint,
                Timestamp = request.Timestamp,
                Score = riskScore,
                Status = riskScore >= 75 ? "blocked" :
                         riskScore >= 40 ? "suspicious" : "clean",
                Reason = string.Join("; ", reasons)
            });

            _db.SaveChanges();

            var finalStatus = riskScore switch
            {
                >= 75 => "blocked",
                >= 40 => "suspicious",
                _ => "clean"
            };

            return new ThreatDetectionResponse
            {
                RiskScore = riskScore,
                Status = finalStatus,
                Reason = string.Join("; ", reasons)
            };
        }

        public async Task<(List<LoginEvent>, int)> GetLogs(HttpContext httpContext, string? ip = null, string? status = null, int page = 1, int pageSize = 50)
        {
            var query = _db.LoginEvents.AsQueryable();

            // Extract user identity from HttpContext
            var user = httpContext.User;
            var isAdmin = user.IsInRole("Admin");
            var userId = user.Identity?.IsAuthenticated == true
                ? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;

            // Limit to own logs if not admin
            if (!isAdmin && !string.IsNullOrWhiteSpace(userId))
            {
                query = query.Where(e => e.UserId == userId);
            }

            // Apply filters
            if (!string.IsNullOrWhiteSpace(ip))
                query = query.Where(e => e.Ip.Contains(ip));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => e.Status == status);

            // Total count before pagination
            var total = await query.CountAsync();

            // Apply pagination
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 50;

            var logs = await query
                .OrderByDescending(e => e.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (logs, total);
        }


    }

}
