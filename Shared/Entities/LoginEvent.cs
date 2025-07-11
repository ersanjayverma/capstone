using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ZTACS.Shared.Entities
{
    public class LoginEvent : Base
    {
        public string UserId { get; set; } = null!;
        private string _ip = string.Empty;


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

        public string Device { get; set; } = null!;
        public string Endpoint { get; set; } = null!;
        public int? Score { get; set; }
        public string Status { get; set; } = "clean"; // default status
        public string Reason { get; set; } = string.Empty; // default reason
        public DateTime Timestamp { get; set; }
    }
}