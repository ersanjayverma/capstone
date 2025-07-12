using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _db.UserProfiles.AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task UpsertFromLoginAsync(ClaimsPrincipal user, ThreatDetectionRequest threatInfo)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return;

            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            var now = DateTime.UtcNow;

            if (profile is null)
            {
                profile = new UserProfile
                {
                    UserId = userId,
                    CreatedAt = now
                };
                _db.UserProfiles.Add(profile);
            }

            profile.UserName = user.Identity?.Name ?? "User";
            profile.Email = user.FindFirst(ClaimTypes.Email)?.Value ?? "";
            profile.FullName = user.FindFirst("name")?.Value;
            profile.FirstName = user.FindFirst("given_name")?.Value;
            profile.LastName = user.FindFirst("family_name")?.Value;
            profile.Locale = user.FindFirst("locale")?.Value;
            profile.Roles = string.Join(',', user.FindAll(ClaimTypes.Role).Select(r => r.Value));

            profile.LastLogin = now;
            profile.UpdatedAt = now;

            profile.LastLoginIp = threatInfo.Ip;
            profile.LastLoginDevice = threatInfo.Device;
            profile.LastLoginEndpoint = threatInfo.Endpoint;
            profile.LastLoginScore = threatInfo.Score;
            profile.LastLoginStatus = threatInfo.Status;
            profile.LastLoginReason = threatInfo.Reason;
            profile.LastLoginCity = threatInfo.City;
            profile.LastLoginCountry = threatInfo.Country;
            profile.LastLoginRegion = threatInfo.Region;
            profile.LastLoginISP = threatInfo.ISP;
            profile.LastLoginASN = threatInfo.ASN;
            profile.IsWhitelisted = threatInfo.IsWhitelisted;
            profile.IsBlocked = threatInfo.IsBlocked;

            await _db.SaveChangesAsync();
        }
    }
}
