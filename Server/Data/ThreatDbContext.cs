using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ZTACS.Shared.Entities;

namespace ZTACS.Server.Data
{
    public class ThreatDbContext : DbContext
    {
        public ThreatDbContext(DbContextOptions<ThreatDbContext> options) : base(options) { }
        public DbSet<BlacklistedIp> BlacklistedIps { get; set; }
        public DbSet<LoginEvent> LoginEvents { get; set; }
    }
}
