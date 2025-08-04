namespace SamsApi.DTOs
{
    public class UserDTO
    {
        public Guid Id
        { get; set; }

        public string UserSecret
        { get; set; }

        public string UserName
        { get; set; }
        public DateTime? CreatedAt
        { get; set; }
        public string role
        { get; set; }
        public bool isActive
        { get; set; }
    }
}
