using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ZTACS.Server.Data;
using ZTACS.Shared.Entities;
using ZTACS.Shared.Models;
using System.Text;

namespace ZTACS.Server.Services
{
    public class ThreatDetectionService : IThreatDetectionService
    {
        private readonly ThreatDbContext _db;
        private readonly HashSet<string> _blacklistedIps;

        public ThreatDetectionService(ThreatDbContext db)
        {
            _db = db;
            _blacklistedIps = _db.BlacklistedIps
                .AsNoTracking()
                .Select(b => b.Ip)
                .ToHashSet();
        }

public ThreatDetectionResponse Analyze(ThreatDetectionRequest request)
{
    var riskScore = 0;
    var reasons = new List<string>();
    var nowUtc = DateTime.UtcNow;

    if (string.IsNullOrWhiteSpace(request.UserId) || request.UserId == "anonymous")
    {
        riskScore += 25;
        reasons.Add("Anonymous user");
    }

    if (_blacklistedIps.Contains(request.Ip))
    {
        riskScore += 80;
        reasons.Add("Blacklisted IP");
    }

    if (request.Endpoint.Contains("/admin", StringComparison.OrdinalIgnoreCase))
    {
        riskScore += 20;
        reasons.Add("Accessed admin endpoint");
    }

    var hour = request.Timestamp.ToUniversalTime().Hour;
    if (hour < 6 || hour > 22)
    {
        riskScore += 10;
        reasons.Add("Unusual access hour");
    }

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
        if (!lastLogins.Any(e => e.Ip == request.Ip))
        {
            riskScore += 20;
            reasons.Add("New IP address");
        }

        if (!lastLogins.Any(e => e.Device == request.Device))
        {
            riskScore += 15;
            reasons.Add("New device used");
        }

        if (lastLogins.Count(e => e.Timestamp > request.Timestamp.AddMinutes(-5)) > 5)
        {
            riskScore += 25;
            reasons.Add("High-frequency login attempts");
        }
    }

    var finalStatus = riskScore switch
    {
        >= 75 => "blocked",
        >= 40 => "suspicious",
        _ => "clean"
    };

    var logEvent = new LoginEvent
    {
        Id = Guid.NewGuid(),
        UserId = request.UserId,
        Ip = request.Ip,
        Device = request.Device,
        Endpoint = request.Endpoint,
        Timestamp = request.Timestamp,
        Score = riskScore,
        Status = finalStatus,
        Reason = string.Join("; ", reasons)
    };

    _db.LoginEvents.Add(logEvent);
    _db.SaveChanges(); // 💾 Save before detail

    return new ThreatDetectionResponse
    {
        RiskScore = riskScore,
        Status = finalStatus,
        Reason = string.Join("; ", reasons),
        LoginEventId = logEvent.Id
    };
}



        public async Task<LogResponse> GetLogs(HttpContext httpContext, string? ip = null, string? status = null,
            int page = 1, int pageSize = 50)
        {
            var query = _db.LoginEvents.AsQueryable();
            var user = httpContext.User;
            //var isAdmin = user.IsInRole("Admin");

            if (!user.Identity?.IsAuthenticated ?? true)
                return new LogResponse();

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //if (!isAdmin && !string.IsNullOrEmpty(userId))
            //{
            //    query = query.Where(e => e.UserId == userId);
            //}

            if (!string.IsNullOrWhiteSpace(ip))
                query = query.Where(e => e.Ip.Contains(ip));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => e.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

            var total = await query.CountAsync();

            if (pageSize <= 0)
            {
                // Return all logs (for export)
                var allLogs = await query
                    .OrderByDescending(e => e.Timestamp)
                    .ToListAsync();

                return new LogResponse
                {
                    Total = total,
                    Logs = allLogs
                };
            }

            // Paginated response (for UI)
            if (page < 1) page = 1;

            var logs = await query
                .OrderByDescending(e => e.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new LogResponse
            {
                Total = total,
                Logs = logs
            };
        }


        public async Task<LogEventDetail?> GetLogDetailAsync(Guid id)
        {
            var log = await _db.LoginEvents.FindAsync(id);
            if (log == null) return null;

            var whois = await EnrichIpAsync(log.Ip);

            return new LogEventDetail
            {
                Id = log.Id,
                UserId = log.UserId,
                IP = log.Ip,
                Device = log.Device,
                Endpoint = log.Endpoint,
                Timestamp = log.Timestamp,
                Score = log.Score,
                Status = log.Status,
                Reason = log.Reason,

                Country = whois.Country ?? string.Empty,
                City = whois.City ?? string.Empty,
                ISP = whois.ISP ?? string.Empty,
                ASN = whois.ASN ?? string.Empty,

                IsWhitelisted = await _db.WhitelistedIps.AnyAsync(w => w.Ip == log.Ip),
                IsBlocked = _blacklistedIps.Contains(log.Ip),
                RequestHeaders = [] // Optional: capture from HttpContext later
            };
        }

        private async Task<(string? Country, string? City, string? ISP, string? ASN)> EnrichIpAsync(string ip)
        {
            try
            {
                using var client = new HttpClient();
                var response =
                    await client.GetAsync($"http://ip-api.com/json/{ip}?fields=status,country,city,isp,as,query");
                if (!response.IsSuccessStatusCode) return default;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.GetProperty("status").GetString() != "success")
                    return default;

                return (
                    Country: root.GetProperty("country").GetString(),
                    City: root.GetProperty("city").GetString(),
                    ISP: root.GetProperty("isp").GetString(),
                    ASN: root.GetProperty("as").GetString()
                );
            }
            catch
            {
                return default;
            }
        }

        public async Task<List<LoginEvent>> GetAllLogs()
        {
            return await _db.LoginEvents
                .OrderByDescending(e => e.Timestamp)
                .ToListAsync();
        }

        public async Task<string> ExportLogsToCsv()
        {
            var logs = await _db.LoginEvents.ToListAsync();
            var csv = new StringBuilder();
            csv.AppendLine("Id,UserId,IP,Device,Endpoint,Score,Status,Reason,Timestamp");

            foreach (var log in logs)
            {
                csv.AppendLine(
                    $"{log.Id},{log.UserId},{log.Ip},{log.Device},{log.Endpoint},{log.Score},{log.Status},{log.Reason},{log.Timestamp:O}");
            }

            return csv.ToString();
        }

        public async Task BlockIp(BlockIpRequest request)
        {
            if (!_blacklistedIps.Contains(request.IP))
            {
                _db.BlacklistedIps.Add(new BlacklistedIp { Ip = request.IP });
                await _db.SaveChangesAsync();
                _blacklistedIps.Add(request.IP);
            }
        }

        public async Task AddToWhitelist(WhitelistIpRequest request)
        {
            var exists = await _db.WhitelistedIps.AnyAsync(x => x.Ip == request.IP);
            if (!exists)
            {
                _db.WhitelistedIps.Add(new WhitelistedIp { Ip = request.IP });
                await _db.SaveChangesAsync();
            }
        }

        public async Task RemoveFromWhitelist(WhitelistIpRequest request)
        {
            var entry = await _db.WhitelistedIps.FirstOrDefaultAsync(x => x.Ip == request.IP);
            if (entry != null)
            {
                _db.WhitelistedIps.Remove(entry);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetWhitelistedIps()
        {
            return await _db.WhitelistedIps
                .Select(w => w.Ip)
                .ToListAsync();
        }

        public async Task<LogStatistics> GetLogStatisticsAsync()
        {
            var total = await _db.LoginEvents.CountAsync();
            var blocked = await _db.LoginEvents.CountAsync(e => e.Status == "blocked");
            var suspicious = await _db.LoginEvents.CountAsync(e => e.Status == "suspicious");
            var clean = total - blocked - suspicious;

            return new LogStatistics
            {
                Total = total,
                Blocked = blocked,
                Suspicious = suspicious,
                Clean = clean
            };
        }
    }
}