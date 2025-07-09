using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ZTACS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires valid JWT
    public class UserController : ControllerBase
    {
        [HttpGet("me")]
        public IActionResult GetUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var userInfo = new
            {
                Name = identity.FindFirst(ClaimTypes.Name)?.Value,
                Email = identity.FindFirst(ClaimTypes.Email)?.Value,
                Subject = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Provider = identity.FindFirst("iss")?.Value,
                Roles = identity.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
            };

            return Ok(userInfo);
        }
    }
}