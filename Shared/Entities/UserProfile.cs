using System.ComponentModel.DataAnnotations;
using ZTACS.Shared.Entities;

namespace ZTACS.Shared.Entities
{
    public class UserProfile : Base
    {
        [Required]
        public string UserId { get; set; } = ""; // From Keycloak 'sub'

        [Required]
        public string UserName { get; set; } = ""; // From 'preferred_username'

        public string Email { get; set; } = "";

        public string? FullName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Locale { get; set; }

        public string? Roles { get; set; } // comma-separated

        public byte[]? ProfileImage { get; set; } // passport-sized image

        public DateTime LastLogin { get; set; } = DateTime.UtcNow;

        // === 🛡️ Threat Intelligence Metadata ===

        public string? LastLoginIp { get; set; }

        public string? LastLoginCity { get; set; }

        public string? LastLoginCountry { get; set; }

        public string? LastLoginRegion { get; set; }

        public string? LastLoginISP { get; set; }

        public string? LastLoginASN { get; set; }

        public string? LastLoginDevice { get; set; }

        public string? LastLoginEndpoint { get; set; }

        public float? LastLoginScore { get; set; }

        public string? LastLoginStatus { get; set; }

        public string? LastLoginReason { get; set; }

        public bool IsWhitelisted { get; set; }

        public bool IsBlocked { get; set; }
    }
}
