using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace ZTACS.Shared.Entities
{
    public abstract class Base
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public static bool IsValidIp(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }
    }
}