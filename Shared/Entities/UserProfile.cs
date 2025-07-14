using System;
using System.ComponentModel.DataAnnotations;

namespace ZTACS.Shared.Entities
{
    public class UserProfile : Base
    {
        // === 🧑 Identity Information ===
        [Required] public string KeycloakId { get; set; } = string.Empty; // From Keycloak sub/subject claim


        public string UserName { get; set; } = string.Empty; // From 'preferred_username'

        public string Email { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Locale { get; set; }

        public string? Roles { get; set; } // Comma-separated roles (e.g., "Admin,User")

        public string? ProfileImage { get; set; } // Passport-sized image blob (base64 stored as byte[])

        public DateTime LastLogin { get; set; } = DateTime.UtcNow;

        // === 🛡️ Threat Intelligence Metadata ===

        public string? LastIp { get; set; }

        public string? LastCity { get; set; }

        public string? LastCountry { get; set; }

        public string? LastRegion { get; set; }

        public string? LastISP { get; set; }

        public string? LastASN { get; set; }

        public string? LastDevice { get; set; }

        public string? LastEndpoint { get; set; }

        public float? LastScore { get; set; }

        public string? LastStatus { get; set; }

        public string? LastReason { get; set; }

        public bool IsWhitelisted { get; set; }

        public bool IsBlocked { get; set; }
    }
}