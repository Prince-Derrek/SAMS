using SamsApi.Models;

namespace SamsApi.Data
{
    public class SeederData
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Roles.Any())
            {
                var role = new List<Role>
                {
                    new Role {Name = "Admin"}
                };
                context.Roles.AddRange(role);
            }

            if (!context.Policies.Any())
            {
                var policies = new List<Policy>
                {
                    new Policy { Name = "CanRegisterUsers"},
                    new Policy { Name = "CanViewUsers"},
                    new Policy { Name = "CanDisableUsers"},
                    new Policy { Name = "CanUpdateUsers"}
                };
                context.Policies.AddRange(policies);
            }

            context.SaveChanges();

            if (!context.RolePolicies.Any())
            {
                var AdminRoleId = context.Roles.First(r => r.Name == "Admin").Id;

                var policies = context.Policies.ToList();

                foreach (var p in policies)
                {
                    context.RolePolicies.Add(new RolePolicy
                    {
                        RoleId = AdminRoleId,
                        PolicyId = p.Id
                    });
                }
                context.SaveChanges();
            }
        }
    }
}
