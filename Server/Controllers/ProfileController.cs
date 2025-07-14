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
            await _profileService.UpsertFromLoginAsync(HttpContext, User, threatRequest);
            return Ok(new { message = "Profile upserted" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfile updatedProfile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null || userId != updatedProfile.KeycloakId)
                return Forbid("User ID mismatch or unauthorized update attempt.");

            var existing = await _profileService.GetByUserIdAsync(userId);
            if (existing is null)
                return NotFound("User profile not found.");

            // Update mutable fields only
            existing.FullName = updatedProfile.FullName;
            existing.FirstName = updatedProfile.FirstName;
            existing.LastName = updatedProfile.LastName;
            existing.ProfileImage = updatedProfile.ProfileImage;
            existing.Locale = updatedProfile.Locale;

            await _profileService.SaveAsync(existing);
            return Ok(existing);
        }
    }
}