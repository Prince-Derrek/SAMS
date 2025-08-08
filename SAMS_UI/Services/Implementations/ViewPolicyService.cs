using Microsoft.EntityFrameworkCore;
using SAMS_UI.Data;
using SAMS_UI.Services.Interfaces;
using System.Security.Claims;

namespace SAMS_UI.Services.Implementations
{
    public class ViewPolicyService : IViewPolicyService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ViewPolicyService> _logger;

        public ViewPolicyService(AppDbContext context, ILogger<ViewPolicyService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<string>> GetPoliciesForCurrentUserAsync(ClaimsPrincipal user)
        {
            var roleName = user.FindFirst(ClaimTypes.Role)?.Value ?? user.FindFirst("role")?.Value;

            if (string.IsNullOrWhiteSpace(roleName))
            {
                _logger.LogWarning("No role claim found for the current user.");
                return new List<string>();
            }

            _logger.LogInformation("Fetching policies for user role: {RoleName}", roleName);

            try
            {
                var policies = await _context.RolePolicies
                    .Include(rp => rp.Policy)
                    .Where(rp => rp.Role.Name == roleName)
                    .Select(rp => rp.Policy.Name)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {PolicyCount} policies for role: {RoleName}", policies.Count, roleName);

                return policies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving policies for role: {RoleName}", roleName);
                return new List<string>();
            }
        }
    }
}
