using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTACS.Shared.Entities;

namespace ZTACS.Shared.Models
{
    public class LogsResponse
    {
        public List<LoginEvent> Logs { get; set; }
        public int Total { get; set; }
    }
}
