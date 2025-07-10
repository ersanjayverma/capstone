using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZTACS.Shared.Models;
using ZTACS.Server.Services;
using ZTACS.Shared.Entities;

namespace ZTACS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThreatDetectionController(
        IThreatDetectionService threatDetectionService,
        ILogger<ThreatDetectionController> logger)
        : ControllerBase
    {
        [HttpPost("detect")]
        public IActionResult DetectThreat([FromBody] ThreatDetectionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = threatDetectionService.Analyze(request);

            logger.LogInformation("Threat analysis for User {UserId}: Risk {RiskScore}, Status {Status}",
                request.UserId, result.RiskScore, result.Status);

            return Ok(result);
        }
        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs()
        {
           var logs =await threatDetectionService.GetLogs();

            return Ok(logs);
        }
    }
}