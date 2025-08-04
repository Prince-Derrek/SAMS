using System.Security;

namespace SamsApi.Models
{
    public class RolePolicy
    {
        public int RoleId
        { get; set; }
        public Role Role
        { get; set; }

        public int PolicyId
        { get; set; }
        public Policy Policies
        { get; set; }
    }
}
