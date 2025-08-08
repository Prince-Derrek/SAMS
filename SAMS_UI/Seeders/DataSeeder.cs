using Microsoft.EntityFrameworkCore;
using SAMS_UI.Data;
using SAMS_UI.Models;

namespace WhatsAppUI.Seeders
{
    public class DataSeeder
    {
        private readonly AppDbContext _context;

        public DataSeeder(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "TechIntern");

            if (role == null)
            {
                role = new Role { Name = "Admin" };
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
            }

            var requiredPolicies = new[]
            {
                "CanViewUsers",
                "CanRegisterUsers",
                "CanManagePolicies",
                "CanManageRoles",
                "CanManagePolicies",
                "CanRegisterUsers",
                "CanViewUserDetails",
                "CanManageRoles",
                "CanDeleteRoles",
                "CanCreateRoles",
                "CanCreatePolicies",
                "CanDeletePolicies",
                "CanDisableUsers",
                "CanAssignPolicies",
                "CanViewAuditLogs"
            };

            foreach (var policyName in requiredPolicies)
            {
                if (!await _context.Policies.AnyAsync(p => p.Name == policyName))
                {
                    _context.Policies.Add(new Policy
                    {
                        Name = policyName,
                        Description = $"Policy: {policyName}"
                    });
                }
            }

            await _context.SaveChangesAsync();

            var existingPolicyIds = await _context.RolePolicies
                .Where(rp => rp.RoleId == role.Id)
                .Select(rp => rp.PolicyId)
                .ToListAsync();
            var allPolicies = await _context.Policies
            .Where(p => requiredPolicies.Contains(p.Name))
            .ToListAsync();

            var newMappings = allPolicies
                .Where(p => !existingPolicyIds.Contains(p.Id))
                .Select(p => new RolePolicy
                {
                    RoleId = role.Id,
                    PolicyId = p.Id
                });

            _context.RolePolicies.AddRange(newMappings);
            await _context.SaveChangesAsync();
        }

    }
}