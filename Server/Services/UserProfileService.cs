using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ZTACS.Server.Data;
using ZTACS.Shared.Entities;
using ZTACS.Shared.Models;

namespace ZTACS.Server.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ThreatDbContext _db;
        private readonly IThreatDetectionService _threatDetectionService;

        public UserProfileService(ThreatDbContext db, IThreatDetectionService threatDetectionService)
        {
            _db = db;
            _threatDetectionService = threatDetectionService;
        }

        public async Task<UserProfile?> GetCurrentProfileAsync(ClaimsPrincipal user)
        {
            var keycloakId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(keycloakId))
                return null;

            return await _db.UserProfiles.FirstOrDefaultAsync(p => p.KeycloakId == keycloakId);
        }

        public async Task<UserProfile?> GetByUserIdAsync(string userId)
        {
            return await _db.UserProfiles.FirstOrDefaultAsync(p => p.KeycloakId == userId);
        }

        public async Task SaveAsync(UserProfile profile)
        {
            profile.UpdatedAt = DateTime.UtcNow;
            _db.UserProfiles.Update(profile);
            await _db.SaveChangesAsync();
        }

        public async Task UpsertFromLoginAsync(HttpContext context, ClaimsPrincipal user, ThreatDetectionRequest request)
        {
            var keycloakId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(keycloakId)) return;

            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.KeycloakId == keycloakId);
            var now = DateTime.UtcNow;

            if (profile == null)
            {
                profile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    KeycloakId = keycloakId,
                    CreatedAt = now
                };
                _db.UserProfiles.Add(profile);
            }

            // Identity fields
            profile.UserName = user.FindFirst("preferred_username")?.Value ?? profile.UserName;
            profile.Email = user.FindFirst(ClaimTypes.Email)?.Value ?? profile.Email;
            profile.FullName = user.FindFirst("name")?.Value ?? profile.FullName;
            profile.FirstName = user.FindFirst("given_name")?.Value ?? profile.FirstName;
            profile.LastName = user.FindFirst("family_name")?.Value ?? profile.LastName;
            profile.Locale = user.FindFirst("locale")?.Value ?? profile.Locale;

            var roles = user.FindAll(ClaimTypes.Role).Select(r => r.Value);
            profile.Roles = string.Join(",", roles);

            profile.LastLogin = request.Timestamp;

            await _threatDetectionService.EnrichProfileFromThreatRequestAsync(context, profile, request);

            profile.UpdatedAt = now;
            await _db.SaveChangesAsync();
        }
    }
}
