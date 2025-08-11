namespace SAMS_UI.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string userName { get; set; }
        public string userSecret { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public bool isActive { get; set; }
    }
}
