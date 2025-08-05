using Microsoft.IdentityModel.Tokens;
using SamsApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SamsApi.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtHelper> _logger;
        public JwtHelper(IConfiguration configuration, ILogger<JwtHelper> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public (string Token, DateTime expiresAt) GenerateToken(User user)
        {
            _logger.LogInformation($"Token Generation has began at {DateTime.UtcNow.AddHours(3)}");
            var secretKey = _configuration["Jwt:Key"];
            _logger.LogInformation("Retrieved Secret Key successfully");

            var issuer = _configuration["Jwt:Issuer"];
            _logger.LogInformation("Retrieved Issuer successfully");

            var audience = _configuration["Jwt:Audience"];
            _logger.LogInformation("Retrieved Audience successfully");

            var expiresInMinutes = Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]);
            _logger.LogInformation("Retrieved Token Duration successfully");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            _logger.LogInformation("Assigning User Claims");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role.Name)

            };
            _logger.LogInformation("Claims assigned Successfully");

            _logger.LogInformation("Creating token");
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: creds
                );
            _logger.LogInformation("Token Created Successfully");

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var expiration = token.ValidTo;

            _logger.LogInformation("Token Generation was successful");

            return (tokenString, expiration);
        }
    }

}
