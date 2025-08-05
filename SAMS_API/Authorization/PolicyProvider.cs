using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SamsApi.Authorization
{
    public class PolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly ILogger<PolicyProvider> _logger;

        const string POLICY_PREFIX = "";
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider
        { get; }
        public PolicyProvider(IOptions<AuthorizationOptions> options, ILogger<PolicyProvider> logger)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
            _logger = logger;
        }
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            _logger.LogInformation($"GetPolicyAsync called for: {policyName}");
            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new PolicyRequirement(policyName));
            return Task.FromResult(policy.Build());
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
            => FallbackPolicyProvider.GetFallbackPolicyAsync();
    }
}
