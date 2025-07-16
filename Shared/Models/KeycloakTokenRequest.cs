using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTACS.Shared.Models
{
    public class KeycloakTokenRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}