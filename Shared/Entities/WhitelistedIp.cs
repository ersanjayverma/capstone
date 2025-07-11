using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTACS.Shared.Entities
{
    public class WhitelistedIp : Base
    {
        public string Ip { get; set; } = default!;
    }
}