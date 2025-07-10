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

            // Log the current event for future audits
            _db.LoginEvents.Add(new LoginEvent
            {
                UserId = request.UserId,
                Ip = request.Ip,
                Device = request.Device,
                Endpoint = request.Endpoint,
                Timestamp = request.Timestamp
            });

            _db.ThreatDetections.Add(new LoginEvent()
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

    }

}
