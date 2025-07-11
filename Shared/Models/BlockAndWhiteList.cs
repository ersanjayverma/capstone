using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ZTACS.Shared.Models
{
    public class BlockIpRequest
    {
        private string _ip = string.Empty;

        [Required]
        public string IP
        {
            get => _ip;
            set
            {
                if (!IsValidIp(value))
                    throw new ValidationException("Invalid IP address.");
                _ip = value;
            }
        }

        private static bool IsValidIp(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }
    }

    public class WhitelistIpRequest
    {
        private string _ip = string.Empty;

        [Required]
        public string IP
        {
            get => _ip;
            set
            {
                if (!IsValidIp(value))
                    throw new ValidationException("Invalid IP address.");
                _ip = value;
            }
        }

        private static bool IsValidIp(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }
    }
}
