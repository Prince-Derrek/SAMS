using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SamsApi.Data;
using System.Security.Claims;

namespace SamsApi.Authorization
{
    public class PolicyHandler : AuthorizationHandler<PolicyRequirement>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PolicyHandler> _logger;

        public PolicyHandler(AppDbContext dbContext, ILogger<PolicyHandler> logger) 
        { 
            _dbContext = dbContext;
            _logger = logger;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyRequirement requirement)
        {
            _logger.LogInformation("Beginning Requirement Handling");
            string[] roles = _dbContext.RolePolicies
                .Include(rp => rp.Role)
                .Include(rp => rp.Policies)
                .Where(rp => rp.Policies.Name == requirement.PolicyName)
                .Select(rp => rp.Role.Name).ToArray();
            _logger.LogInformation("Role and its policies loaded from the Db");

            _logger.LogInformation("Validating user policy claims based on their role");
            Claim? userRoleClaim = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
            if (userRoleClaim != null && roles.Contains(userRoleClaim.Value))
            {
                _logger.LogInformation("User role claim successful");
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogInformation("User role claim validation failed");
            }

                return Task.CompletedTask;
        }
    }
}
