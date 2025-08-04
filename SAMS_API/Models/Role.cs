namespace SamsApi.Models
{
    public class Role
    {
        public int Id
        { get; set; }
        public string Name
        { get; set; }

        //Navigation
        public ICollection<User> Users
        { get; set; }
        public ICollection<RolePolicy> RolePolicies
        { get; set; }
    }
}
