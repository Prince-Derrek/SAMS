using SAMS_UI.Data;
using SAMS_UI.Models;
using SAMS_UI.Services.Interfaces;
using SAMS_UI.ViewModels;

namespace SAMS_UI.Services.Implementations
{
    public class RegisterUserService : IRegisterUserService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RegisterUserService> _logger;

        public RegisterUserService(AppDbContext context, ILogger<RegisterUserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GenerateSecret()
        {
            var secret = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "")
                .Replace("+", "")
                .Substring(0, 16);

            _logger.LogDebug("Generated new user secret");
            return secret;
        }

        public async Task<bool> CreateUserAsync(RegisterViewModel user)
        {
            try
            {
                _logger.LogInformation("Attempting to register user locally in frontend DB.");

                if (_context.Users.Any(u => u.Username == user.userName))
                {
                    _logger.LogWarning("User with same username already exists.");
                    return false;
                }

                // Create user entity
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = user.userName,
                    UserSecret = GenerateSecret(),
                    RoleId = user.RoleId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User registration successful.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User registration failed.");
                return false;
            }
        }
    }
}
