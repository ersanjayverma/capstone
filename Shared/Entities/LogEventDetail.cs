using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZTACS.Shared.Entities
{
    public class LogEventDetail : Base
    {
        [ForeignKey("LoginEvent")]
        public Guid LoginEventId { get; set; }
        public LoginEvent? LoginEvent { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string IP { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public int? Score { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }

        // 🌍 Enrichment
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string ISP { get; set; } = string.Empty;
        public string ASN { get; set; } = string.Empty;

        public string UserAgent { get; set; } = string.Empty;

        // For EF Core 6+: use string serialization for arrays
        public string? RequestHeadersJson { get; set; }

        [NotMapped]
        public string[] RequestHeaders
        {
            get => string.IsNullOrWhiteSpace(RequestHeadersJson)
                ? Array.Empty<string>()
                : System.Text.Json.JsonSerializer.Deserialize<string[]>(RequestHeadersJson) ?? Array.Empty<string>();
            set => RequestHeadersJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        // Flags
        public bool IsWhitelisted { get; set; }
        public bool IsBlocked { get; set; }
    }
}
