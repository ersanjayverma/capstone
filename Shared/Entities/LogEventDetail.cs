using System;

namespace ZTACS.Shared.Entities
{
    public class LogEventDetail:Base
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string IP { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public int? Score { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }

        // 🆕 Additional Enrichment
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ISP { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string[] RequestHeaders { get; set; } = [];
        public string ASN { get; set; } = string.Empty;

        // Convenience flags
        public bool IsWhitelisted { get; set; }
        public bool IsBlocked { get; set; }
    }
}
