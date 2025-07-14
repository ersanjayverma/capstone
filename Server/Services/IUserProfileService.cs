using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ZTACS.Server.Data;
using ZTACS.Shared.Entities;
using ZTACS.Shared.Models;
using System.Text;

namespace ZTACS.Server.Services
{
    public interface IUserProfileService
    {
        Task<UserProfile?> GetCurrentProfileAsync(ClaimsPrincipal user);
        Task<UserProfile?> GetByUserIdAsync(string userId);
        Task SaveAsync(UserProfile profile);
        Task UpsertFromLoginAsync(HttpContext context, ClaimsPrincipal user, ThreatDetectionRequest request);
    }
}