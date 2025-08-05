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
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext context, JwtHelper jwtHelper, ILogger<AuthController> logger)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDto)
        {
            _logger.LogInformation($"{loginDto.Id} tried to Login at {DateTime.UtcNow.AddHours(3)} ");
            
            if (!Guid.TryParse(loginDto.Id, out Guid parsedUserId))
            {
                _logger.LogInformation($"{loginDto.Id} is an invalid Id format or does not exist!");
                return Unauthorized("Invalid user ID format.");
            }

            var user = await _context.Users
             .Include(u => u.Role)
                .ThenInclude(r => r.RolePolicies)
                  .ThenInclude(rp => rp.Policies)
              .FirstOrDefaultAsync(u => u.Id == parsedUserId && u.isActive);
            
            if (user == null)
            {
                _logger.LogInformation("User could not be found or is inactive");
                return Unauthorized("User not found or inactive.");
            }
            else
            {
                _logger.LogInformation("User found in the Db");
            }

            if (user.UserSecret != loginDto.UserSecret)
            {
                _logger.LogInformation("Invalid Credentials Provided");
                return Unauthorized("Invalid credentials.");
            }
                


            var (token, expiration) = _jwtHelper.GenerateToken(user);
            _logger.LogInformation("Token Generated Successfully");

            var response = new LoginResponseDTO
            {
                Token = token,
                expiresAt = expiration
            };

            return Ok(response);
        }
    }
}
