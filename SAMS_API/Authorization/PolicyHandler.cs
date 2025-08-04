using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SamsApi.Data;
using System.Security.Claims;

namespace SamsApi.Authorization
{
    public class PolicyHandler : AuthorizationHandler<PolicyRequirement>
    {
        private readonly AppDbContext _dbContext;

        public PolicyHandler(AppDbContext dbContext) { _dbContext = dbContext; }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyRequirement requirement)
        {
            string[] roles = _dbContext.RolePolicies
                .Include(rp => rp.Role)
                .Include(rp => rp.Policies)
                .Where(rp => rp.Policies.Name == requirement.PolicyName)
                .Select(rp => rp.Role.Name).ToArray();

            Claim? userRoleClaim = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
            if (userRoleClaim != null && roles.Contains(userRoleClaim.Value))
            {

                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
