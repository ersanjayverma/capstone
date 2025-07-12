using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using ZTACS.Server.Services;
using ZTACS.Shared.Entities;
using ZTACS.Shared.Models;

namespace ZTACS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserProfileService _profileService;

        public ProfileController(IUserProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<ActionResult<UserProfile>> Get()
        {
            var profile = await _profileService.GetCurrentProfileAsync(User);
            return profile is null ? NotFound() : Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] ThreatDetectionRequest threatRequest)
        {
            await _profileService.UpsertFromLoginAsync(User, threatRequest);
            return Ok(new { message = "Profile upserted" });
        }
    }
}
