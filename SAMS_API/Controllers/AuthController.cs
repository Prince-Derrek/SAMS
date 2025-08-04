using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamsApi.Data;
using SamsApi.DTOs;
using SamsApi.Helpers;

namespace SamsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthController(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDto)
        {
            if (!Guid.TryParse(loginDto.Id, out Guid parsedUserId))
                return Unauthorized("Invalid user ID format.");
            var user = await _context.Users
             .Include(u => u.Role)
                .ThenInclude(r => r.RolePolicies)
                  .ThenInclude(rp => rp.Policies)
              .FirstOrDefaultAsync(u => u.Id == parsedUserId && u.isActive);

            if (user == null)
                return Unauthorized("User not found or inactive.");

            if (user.UserSecret != loginDto.UserSecret)
                return Unauthorized("Invalid credentials.");


            var (token, expiration) = _jwtHelper.GenerateToken(user);

            var response = new LoginResponseDTO
            {
                Token = token,
                expiresAt = expiration
            };

            return Ok(response);
        }
    }
}
