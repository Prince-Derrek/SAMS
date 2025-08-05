using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SAMS_UI.Data;
using System.Security.Claims;

namespace SAMS_UI.Authorization
{
    public class PolicyHandler : AuthorizationHandler<PolicyRequirement>
    {
        private readonly AppDbContext _db;
        private readonly ILogger<PolicyHandler> _logger;

        public PolicyHandler(AppDbContext db, ILogger<PolicyHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PolicyRequirement requirement)
        {
            _logger.LogInformation("Beginning Requirement Handling");
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
