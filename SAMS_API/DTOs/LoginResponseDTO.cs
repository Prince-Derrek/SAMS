namespace SamsApi.DTOs
{
    public class LoginResponseDTO
    {
        public string Token
        { get; set; } = null!;
        public DateTime expiresAt
        { get; set; }
    }
}
