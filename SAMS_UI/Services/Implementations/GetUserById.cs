using SAMS_UI.Data;
using SAMS_UI.Services.Interfaces;
using SAMS_UI.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace SAMS_UI.Services.Implementations
{
    public class GetUserById : IGetUserById
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GetUserById> _logger;

        public GetUserById(AppDbContext context, ILogger<GetUserById> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserViewModel> GetUserByIdAsync(Guid id)
        {
            _logger.LogInformation("Querying frontend database for user with Id {UserId}", id);

            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role) // Ensure role is loaded
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                _logger.LogWarning("No user found in frontend database with Id {UserId}", id);
                return new UserViewModel();
            }

            _logger.LogInformation("User found in frontend database with Id {UserId}", id);

            return new UserViewModel
            {
                Id = user.Id,
                UserName = user.Username,
                UserSecret = user.UserSecret,
                Role = user.Role?.Name,
                isActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
