using Microsoft.EntityFrameworkCore;
using SAMS_UI.Models;

namespace SAMS_UI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Policy> Policies => Set<Policy>();
        public DbSet<RolePolicy> RolePolicies => Set<RolePolicy>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

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
