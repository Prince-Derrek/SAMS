using System.ComponentModel.DataAnnotations;

namespace SamsApi.Models
{
    public class User
    {
        [Key]
        public Guid Id
        { get; set; }

        [Required]
        public string UserName
        { get; set; }

        [Required]
        public string UserSecret
        { get; set; }
        public Role Role
        { get; set; }
        public DateTime CreatedAt
        { get; set; } = DateTime.UtcNow;
        public string Description
        { get; set; }
        public bool isActive
        { get; set; } = true;
        public int RoleId
        { get; set; }
    }
}
