namespace SAMS_UI.DTOs
{
    public class AssignPolicyToRoleDTO
    {
        public int RoleId
        { get; set; }
        public List<int> PolicyIds
        { get; set; } = new();
    }
}
