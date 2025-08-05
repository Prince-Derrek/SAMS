using Microsoft.EntityFrameworkCore;
using SAMS_UI.Models;

namespace SAMS_UI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Policy> Policies => Set<Policy>();
        public DbSet<RolePolicy> RolePolicies => Set<RolePolicy>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RolePolicy>()
                .HasKey(rp => new { rp.RoleId, rp.PolicyId });

            modelBuilder.Entity<RolePolicy>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePolicies)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePolicy>()
                .HasOne(rp => rp.Policy)
                .WithMany(p => p.RolePolicies)
                .HasForeignKey(rp => rp.PolicyId);
        }
    }
}
