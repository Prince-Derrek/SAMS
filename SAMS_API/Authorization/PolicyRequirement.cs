using Microsoft.AspNetCore.Authorization;

namespace SamsApi.Authorization
{
    public class PolicyRequirement : IAuthorizationRequirement
    {
        public string PolicyName
        { get; }

        public PolicyRequirement(string policyName)
        {
            PolicyName = policyName;
        }
    }
}
