using Microsoft.AspNetCore.Authorization;

namespace SAMS_UI.Authorization
{
    public class PolicyRequirement : IAuthorizationRequirement
    {
        public string PolicyName { get; set; }

        public PolicyRequirement(string policyName)
        {
            PolicyName = policyName;
        }
    }
}
