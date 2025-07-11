using ZTACS.Shared.Entities;
using ZTACS.Shared.Models;

namespace ZTACS.Server.Services
{
    public interface IThreatDetectionService
    {
        ThreatDetectionResponse Analyze(ThreatDetectionRequest request);

        Task<LogResponse> GetLogs(HttpContext httpContext, string? ip = null,
            string? status = null, int page = 1,
            int pageSize = 50);
    }
}