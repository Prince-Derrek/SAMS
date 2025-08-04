namespace SamsApi.DTOs
{
    public class LoginRequestDTO
    {
        public string userId
        { get; set; } = null!;
        public string userSecret
        { get; set; } = null!;
    }
}
