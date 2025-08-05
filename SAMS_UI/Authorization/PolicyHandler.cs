using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SAMS_UI.Data;
using System.Security.Claims;

namespace SAMS_UI.Authorization
{
    public class PolicyHandler : AuthorizationHandler<PolicyRequirement>
    {
        private readonly AppDbContext _db;

        public PolicyHandler(AppDbContext db)
        {
            _db = db;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PolicyRequirement requirement)
        {
            if (!(context.User.Identity?.IsAuthenticated ?? false))
                return;

            var roleName =
                context.User.FindFirst(ClaimTypes.Role)?.Value ??
                context.User.FindFirst("role")?.Value;

            if (string.IsNullOrWhiteSpace(roleName))
                return;

            var hasPermission = await _db.RolePolicies
                .Include(rp => rp.Role)
                .Include(rp => rp.Policy)
                .AnyAsync(rp =>
                    rp.Role.Name == roleName &&
                    rp.Policy.Name == requirement.PolicyName);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}
