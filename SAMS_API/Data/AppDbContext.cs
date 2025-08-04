using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using SamsApi.Models;

namespace SamsApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users
        { get; set; }
        public DbSet<Role> Roles
        { get; set; }
        public DbSet<Policy> Policies
        { get; set; }
        public DbSet<RolePolicy> RolePolicies
        { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<RolePolicy>()
                .HasKey(rp => new
                {
                    rp.RoleId,
                    rp.PolicyId
                });
            modelBuilder.Entity<RolePolicy>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePolicies)
                .HasForeignKey(rp => rp.RoleId);
            modelBuilder.Entity<RolePolicy>()
                .HasOne(rp => rp.Policies)
                .WithMany(p => p.RolePolicies)
                .HasForeignKey(rp => rp.PolicyId);
        }
    }
}
