using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTACS.Shared.Entities
{
    public class BlacklistedIp : Base
    {
        public string Ip { get; set; } = null!;
    }
}