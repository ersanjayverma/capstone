using ZTACS.Shared.Models;

namespace ZTACS.Server.Services
{
    public class ThreatDetectionService : IThreatDetectionService
    {
        private readonly HashSet<string> _blacklistedIps = new() { "10.0.0.5", "198.51.100.23" };

        public ThreatDetectionResponse Analyze(ThreatDetectionRequest request)
        {
            var riskScore = 0;
            var reasons = new List<string>();

            if (_blacklistedIps.Contains(request.Ip))
            {
                riskScore += 80;
                reasons.Add("Blacklisted IP");
            }

            if (request.Endpoint.Contains("/admin"))
            {
                riskScore += 20;
                reasons.Add("Admin endpoint accessed");
            }

            var hour = request.Timestamp.ToUniversalTime().Hour;
            if (hour < 6 || hour > 22)
            {
                riskScore += 10;
                reasons.Add("Access at unusual time");
            }

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