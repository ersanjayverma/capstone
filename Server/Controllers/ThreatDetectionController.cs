using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZTACS.Shared.Models;
using ZTACS.Server.Services;

namespace ZTACS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThreatDetectionController : ControllerBase
    {
        private readonly IThreatDetectionService _threatDetectionService;
        private readonly ILogger<ThreatDetectionController> _logger;

        public ThreatDetectionController(IThreatDetectionService threatDetectionService, ILogger<ThreatDetectionController> logger)
        {
            _threatDetectionService = threatDetectionService;
            _logger = logger;
        }

        [HttpPost("detect")]
        public IActionResult DetectThreat([FromBody] ThreatDetectionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _threatDetectionService.Analyze(request);

            _logger.LogInformation("Threat analysis for User {UserId}: Risk {RiskScore}, Status {Status}",
                request.UserId, result.RiskScore, result.Status);

            return Ok(result);
        }
    }
}