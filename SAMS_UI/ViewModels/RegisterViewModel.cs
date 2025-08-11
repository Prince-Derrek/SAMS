using SAMS_UI.Models;

namespace SAMS_UI.ViewModels
{
    public class RegisterViewModel
    {
        public string userName { get; set; }
        public string? Description { get; set; }
        public int? RoleId { get; set; }

        // New property for the dropdown
        public List<Role> AvailableRoles { get; set; } = new();
    }
}
