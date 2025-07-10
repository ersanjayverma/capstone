using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTACS.Shared.Entities
{
    public class LoginEvent: Base
    {
        public string UserId { get; set; } = null!;
        public string Ip { get; set; } = null!;
        public string Device { get; set; } = null!;
        public string Endpoint { get; set; } = null!;
        public int? Score { get; set; }
        public string Status { get; set; } = "clean"; // default status
        public string Reason { get; set; } = string.Empty; // default reason
        public DateTime Timestamp { get; set; }
    }

}
