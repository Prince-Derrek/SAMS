namespace SAMS_UI.Models
{
    public class Policy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<RolePolicy> RolePolicies { get; set; }
    }
}
