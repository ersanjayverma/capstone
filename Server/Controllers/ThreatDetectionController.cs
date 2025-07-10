using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZTACS.Server.Services;
using ZTACS.Shared.Entities;
using ZTACS.Shared.Models;

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
        public async Task<ActionResult<List<LoginEvent>>> GetLogs([FromQuery] string? ip = null, [FromQuery] string? status = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var logs = await threatDetectionService.GetLogs(HttpContext, ip, status, page, pageSize);
            return Ok(logs);
        }
    }
}