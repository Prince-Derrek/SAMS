using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SamsApi.Authorization
{
    public class PolicyProvider : IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "";
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider
        { get; }
        public PolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
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
