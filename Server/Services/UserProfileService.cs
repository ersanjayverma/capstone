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

        public UserProfileService(ThreatDbContext db)
        {
            _db = db;
        }

        public async Task<UserProfile?> GetCurrentProfileAsync(ClaimsPrincipal user)
        {
            var keycloakId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(keycloakId))
                return null;

            return await _db.UserProfiles.FirstOrDefaultAsync(p => p.KeycloakId == keycloakId);
        }

        public async Task UpsertFromLoginAsync(ClaimsPrincipal user, ThreatDetectionRequest request)
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
                    CreatedAt = now,
                };
                _db.UserProfiles.Add(profile);
            }

            // ✅ Basic identity from Keycloak
            profile.UserName = user.FindFirst("preferred_username")?.Value ?? profile.UserName;
            profile.Email = user.FindFirst(ClaimTypes.Email)?.Value ?? profile.Email;
            profile.FullName = user.FindFirst("name")?.Value ?? profile.FullName;
            profile.FirstName = user.FindFirst("given_name")?.Value ?? profile.FirstName;
            profile.LastName = user.FindFirst("family_name")?.Value ?? profile.LastName;
            profile.Locale = user.FindFirst("locale")?.Value ?? profile.Locale;

            var roles = user.FindAll(ClaimTypes.Role).Select(r => r.Value);
            profile.Roles = string.Join(",", roles);

            profile.LastLogin = request.Timestamp;

            // ✅ From ThreatDetectionRequest (must be enriched before calling this)
            profile.LastIp = request.Ip;
            profile.LastDevice = request.Device;
            profile.LastEndpoint = request.Endpoint;

            profile.LastScore = request.Score;
            profile.LastStatus = request.Status;
            profile.LastReason = request.Reason;

            profile.LastCity = request.City;
            profile.LastCountry = request.Country;
            profile.LastRegion = request.Region;
            profile.LastISP = request.ISP;
            profile.LastASN = request.ASN;

            profile.IsWhitelisted = request.IsWhitelisted;
            profile.IsBlocked = request.IsBlocked;

            profile.UpdatedAt = now;

            await _db.SaveChangesAsync();
        }
    }
}
