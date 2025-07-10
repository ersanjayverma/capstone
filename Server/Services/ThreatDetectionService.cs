using Microsoft.EntityFrameworkCore;
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

            // 1. Static: IP blacklist
            if (_blacklistedIps.Contains(request.Ip))
            {
                riskScore += 80;
                reasons.Add("Blacklisted IP");
            }

            // 2. Endpoint targeting
            if (request.Endpoint.Contains("/admin"))
            {
                riskScore += 20;
                reasons.Add("Accessed admin endpoint");
            }

            // 3. Odd hour access
            var hour = request.Timestamp.ToUniversalTime().Hour;
            if (hour < 6 || hour > 22)
            {
                riskScore += 10;
                reasons.Add("Unusual access hour");
            }

            // 4. Database lookups:
            var lastLogins = _db.LoginEvents
                .Where(e => e.UserId == request.UserId)
                .OrderByDescending(e => e.Timestamp)
                .Take(10)
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

                // 5. Brute-force pattern (5+ logins in <5 mins)
                var recentWindow = lastLogins
                    .Where(e => e.Timestamp > request.Timestamp.AddMinutes(-5))
                    .Count();

                if (recentWindow > 5)
                {
                    riskScore += 25;
                    reasons.Add("High-frequency login attempts");
                }
            }

            // Log current event for future analysis
            _db.LoginEvents.Add(new LoginEvent
            {
                UserId = request.UserId,
                Ip = request.Ip,
                Device = request.Device,
                Endpoint = request.Endpoint,
                Timestamp = request.Timestamp,
                
            });
            _db.SaveChanges();

            // Final status
            var status = riskScore switch
            {
                >= 75 => "blocked",
                >= 40 => "suspicious",
                _ => "clean"
            };

            return new ThreatDetectionResponse
            {
                RiskScore = riskScore,
                Status = status,
                Reason = string.Join("; ", reasons)
            };
        }
    }

}
