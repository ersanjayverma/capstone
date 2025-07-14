using Microsoft.AspNetCore.Http;
using ZTACS.Shared.Entities;
using ZTACS.Shared.Models;
using System.Text;

namespace ZTACS.Server.Services
{
    public interface IThreatDetectionService
    {
        ThreatDetectionResponse Analyze(HttpContext httpContext, ThreatDetectionRequest request);

        Task<LogResponse> GetLogs(
            HttpContext httpContext,
            string? ip = null,
            string? status = null,
            int page = 1,
            int pageSize = 50
        );

        Task<LogEventDetail?> GetLogDetailAsync(Guid id);

        Task<List<LoginEvent>> GetAllLogs();

        Task EnrichProfileFromThreatRequestAsync(HttpContext httpContext, UserProfile profile,
            ThreatDetectionRequest request);


        Task<string> ExportLogsToCsv();

        Task BlockIp(BlockIpRequest request);

        Task AddToWhitelist(WhitelistIpRequest request);

        Task RemoveFromWhitelist(WhitelistIpRequest request);
        string ExtractClientIp(HttpContext context);
        Task<List<string>> GetWhitelistedIps();

        Task<LogStatistics> GetLogStatisticsAsync();
    }
}