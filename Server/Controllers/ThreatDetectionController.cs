using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZTACS.Server.Services;
using ZTACS.Shared.Entities;
using ZTACS.Shared.Models;
using System.Text;

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
        public async Task<IActionResult> GetLogs([FromQuery] string? ip = null, [FromQuery] string? status = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var logs = await threatDetectionService.GetLogs(HttpContext, ip, status, page, pageSize);
            return Ok(new  LogsResponse()
        {
            Logs = logs.Item1,
            Total = logs.Item2
            });
        }

        [HttpGet("logs/{id}")]
        public async Task<IActionResult> GetLogById(Guid id)
        {
            var log = await threatDetectionService.GetLogDetailAsync(id);
            if (log == null)
                return NotFound();

            return Ok(log);
        }

        [HttpGet("logs/export")]
        public async Task<IActionResult> ExportLogs()
        {
            var csv = await threatDetectionService.ExportLogsToCsv();
            var bytes = Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "threat_logs.csv");
        }

        [HttpPost("block-ip")]
        public async Task<IActionResult> BlockIp([FromBody] BlockIpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IP) || !IsValidIp(request.IP))
                return BadRequest("Invalid IP");

            await threatDetectionService.BlockIp(request);
            return Ok();
        }

        [HttpGet("logs/stats")]
        public async Task<IActionResult> GetLogStatistics()
        {
            var stats = await threatDetectionService.GetLogStatisticsAsync();
            return Ok(stats);
        }

        [HttpPost("whitelist/add")]
        public async Task<IActionResult> AddToWhitelist([FromBody] WhitelistIpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IP) || !IsValidIp(request.IP))
                return BadRequest("Invalid IP");

            await threatDetectionService.AddToWhitelist(request);
            return Ok();
        }

        [HttpPost("whitelist/remove")]
        public async Task<IActionResult> RemoveFromWhitelist([FromBody] WhitelistIpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IP) || !IsValidIp(request.IP))
                return BadRequest("Invalid IP");

            await threatDetectionService.RemoveFromWhitelist(request);
            return Ok();
        }

        [HttpGet("whitelist")]
        public async Task<IActionResult> GetWhitelistedIps()
        {
            var ips = await threatDetectionService.GetWhitelistedIps();
            return Ok(ips);
        }

        private static bool IsValidIp(string ip)
        {
            return System.Net.IPAddress.TryParse(ip, out _);
        }
    }
}
