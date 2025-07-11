using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ZTACS.Shared.Entities;

namespace ZTACS.Server.Data
{
    public class ThreatDbContext : DbContext
    {
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Base && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var baseEntity = (Base)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    baseEntity.CreatedAt = DateTime.UtcNow;
                }

                baseEntity.UpdatedAt = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }

        public ThreatDbContext(DbContextOptions<ThreatDbContext> options) : base(options)
        {
        }

        public DbSet<BlacklistedIp> BlacklistedIps { get; set; }
        public DbSet<LoginEvent> LoginEvents { get; set; }
        public DbSet<LogEventDetail> LogEventDetails { get; set; }
        public DbSet<WhitelistedIp> WhitelistedIps { get; set; } = default!;
    }
}