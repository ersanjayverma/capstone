using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTACS.Shared.Entities
{
    public class LoginEvent
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Ip { get; set; }
        public string Device { get; set; }
        public string Endpoint { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
