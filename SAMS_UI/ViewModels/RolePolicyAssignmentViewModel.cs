namespace SAMS_UI.ViewModels
{
    public class RolePolicyAssignmentViewModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public string PolicyName { get; set; }
        public int PolicyId { get; set; }
        public List<PolicyViewModel> AllPolicies { get; set; } = new();
        public List<int> AssignedPolicyIds { get; set; } = new();
    }
}
