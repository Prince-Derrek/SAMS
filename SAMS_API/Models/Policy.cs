namespace SamsApi.Models
{
    public class Policy
    {
        public int Id
        { get; set; }
        public string Name
        { get; set; }

        //Navigation
        public ICollection<RolePolicy> RolePolicies
        { get; set; }
    }
}
