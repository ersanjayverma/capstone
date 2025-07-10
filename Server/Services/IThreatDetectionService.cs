using ZTACS.Shared.Entities;
using ZTACS.Shared.Models;

namespace ZTACS.Server.Services
{
    public interface IThreatDetectionService
    {
        ThreatDetectionResponse Analyze(ThreatDetectionRequest request);
        Task<List<LoginEvent>> GetLogs();
    }
}