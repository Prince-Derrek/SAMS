using System.ComponentModel.DataAnnotations;

namespace SAMS_UI.Models
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string UserSecret { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
