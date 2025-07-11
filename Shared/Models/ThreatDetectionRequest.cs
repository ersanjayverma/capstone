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
                private static bool IsValidIp(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }
    }
}