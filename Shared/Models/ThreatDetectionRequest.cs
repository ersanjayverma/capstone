using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ZTACS.Shared.Models
{
    public class ThreatDetectionRequest
    {
        [Required] public string UserId { get; set; }
        private string _ip = string.Empty;

        [Required]
        public string Ip
        {
            get => _ip;
            set
            {
                if (!IsValidIp(value))
                    throw new ValidationException("Invalid IP address.");
                _ip = value;
            }
        }

        [Required] public string Device { get; set; }
        [Required] public string Endpoint { get; set; }
        [Required] public DateTime Timestamp { get; set; }
public float Score { get; set; }
public string Status { get; set; } = "";
public string Reason { get; set; } = "";
public string City { get; set; } = "";
public string Country { get; set; } = "";
public string Region { get; set; } = "";
public string ISP { get; set; } = "";
public string ASN { get; set; } = "";
public bool IsWhitelisted { get; set; }
public bool IsBlocked { get; set; }

        private static bool IsValidIp(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }
    }
}