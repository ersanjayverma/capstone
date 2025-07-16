using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ZTACS.API.Data;
using ZTACS.Shared.Entities;
using ZTACS.Shared.Models;

namespace ZTACS.API.Services
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

        public ThreatDetectionResponse Analyze(HttpContext context, ThreatDetectionRequest request)
        {
            request.Ip = ExtractClientIp(context);
            var riskScore = 0;
            var reasons = new List<string>();

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

            var lastLogins = _db.LogEvents
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

                if (lastLogins.Count(e => e.Timestamp > request.Timestamp.AddMinutes(-1)) > 15)
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

            var logEvent = new LogEvent
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


            _db.LogEvents.Add(logEvent);
            _db.SaveChanges();
            var details = GetLogDetailAsync(logEvent.Id).GetAwaiter().GetResult();
            if (details is not null)
            {
                details.LoginEventId = logEvent.Id;
                _db.LogEventDetails.Add(details);
                _db.SaveChanges();
            }

            return new ThreatDetectionResponse
            {
                RiskScore = riskScore,
                Status = finalStatus,
                Reason = logEvent.Reason,
                LoginEventId = logEvent.Id
            };
        }

        public async Task EnrichProfileFromThreatRequestAsync(HttpContext context, UserProfile profile,
            ThreatDetectionRequest request)
        {
            request.Ip = ExtractClientIp(context);
            profile.LastIp = request.Ip;
            profile.LastDevice = request.Device;
            profile.LastEndpoint = request.Endpoint;
            profile.LastScore = request.Score;
            profile.LastStatus = request.Status;
            profile.LastReason = request.Reason;


            // Always enrich
            var (country, city, isp, asn) = await EnrichIpAsync(request.Ip);

            profile.LastCity = city ?? "";
            profile.LastCountry = country ?? "";
            profile.LastISP = isp ?? "";
            profile.LastASN = asn ?? "";
            profile.LastRegion = request.Region ?? "";

            profile.IsWhitelisted = request.IsWhitelisted;
            profile.IsBlocked = request.IsBlocked;
        }


        public async Task<(string? Country, string? City, string? ISP, string? ASN)> EnrichIpAsync(string ip)
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

        public async Task<LogResponse> GetLogs(HttpContext httpContext, string? ip = null, string? status = null,
            int page = 1, int pageSize = 50)
        {
            var query = _db.LogEvents.AsQueryable();
            var user = httpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
                return new LogResponse();

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!string.IsNullOrWhiteSpace(ip))
                query = query.Where(e => e.Ip.Contains(ip));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => e.Status.ToLower().Equals(status.ToLower()));

            var total = await query.CountAsync();

            if (pageSize <= 0)
            {
                var allLogs = await query
                    .OrderByDescending(e => e.Timestamp)
                    .ToListAsync();

                return new LogResponse
                {
                    Total = total,
                    Logs = allLogs
                };
            }

            if (page < 1) page = 1;

            var logs = await query.Where(x => x.UserId ==userId)
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
            var log = await _db.LogEvents.FindAsync(id);
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
                RequestHeaders = [] // for future use
            };
        }

        public async Task<List<LogEvent>> GetAllLogs(HttpContext ctx)
        {
            var user = ctx.User;
            return await _db.LogEvents.Where(x => x.UserId == user.FindFirst(ClaimTypes.NameIdentifier).Value).ToListAsync();
        }

        public async Task<string> ExportLogsToCsv(HttpContext ctx)
        {
            var user = ctx.User;
            var logs = await _db.LogEventDetails.Where(x=>x.UserId == user.FindFirst(ClaimTypes.NameIdentifier).Value).ToListAsync();
            var csv = new StringBuilder();

            // Header
            csv.AppendLine(
                "LoginEventId,UserId,IP,Device,Endpoint,Score,Status,Reason,Timestamp,Country,City,Region,ISP,ASN,UserAgent,IsWhitelisted,IsBlocked");

            foreach (var log in logs)
            {
                csv.AppendLine(string.Join(",",
                    Escape(log.LoginEventId.ToString()),
                    Escape(log.UserId),
                    Escape(log.IP),
                    Escape(log.Device),
                    Escape(log.Endpoint),
                    log.Score?.ToString() ?? "",
                    Escape(log.Status),
                    Escape(log.Reason),
                    Escape(((long)(new DateTimeOffset(log.Timestamp).ToUnixTimeMilliseconds()/1000)).ToString()),
                    Escape(log.Country),
                    Escape(log.City),
                    Escape(log.Region),
                    Escape(log.ISP),
                    Escape(log.ASN),
                    Escape(log.UserAgent),
                    log.IsWhitelisted.ToString(),
                    log.IsBlocked.ToString()
                ));
            }

            return csv.ToString();
        }

        // 🔐 Escapes commas, quotes, and newlines for CSV-safe output
        private static string Escape(string? field)
        {
            if (string.IsNullOrWhiteSpace(field))
                return "";

            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                // Double all quotes, wrap in quotes
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
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

        public string ExtractClientIp(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                var ip = forwardedFor.ToString().Split(',')[0].Trim();
                if (!string.IsNullOrWhiteSpace(ip))
                    return ip;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        }

        public async Task<LogStatistics> GetLogStatisticsAsync()
        {
            var total = await _db.LogEvents.CountAsync();
            var blocked = await _db.LogEvents.CountAsync(e => e.Status == "blocked");
            var suspicious = await _db.LogEvents.CountAsync(e => e.Status == "suspicious");
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