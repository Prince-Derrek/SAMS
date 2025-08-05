using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamsApi.DTOs;
using SamsApi.Services.Interfaces;

namespace SamsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [Authorize(Policy = "CanRegisterUsers")]
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterUserDTO dto)
        {
            _logger.LogInformation("Registering new user");
            var result = await _userService.RegisterUserAsync(dto);

            _logger.LogInformation($"User registered successfully");
            return Ok(result);
        }

        [Authorize(Policy = "CanViewUsers")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResponseDTO<UserDTO>>> GetPaginatedUsersAsync([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting the List of all users");
            var users = await _userService.GetPaginatedUsersAsync(pageIndex, pageSize);

            _logger.LogInformation("List retrieved successfully");
            return Ok(users);
        }

        [Authorize(Policy = "CanViewUsers")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserByID(Guid id)
        {
            _logger.LogInformation("Getting specific user");
            var user = await _userService.GetUserByIdAsync(id);

            _logger.LogInformation("User retrieved successfully");
            return Ok(user);
        }

        [Authorize(Policy = "CanDisableUsers")]
        [HttpPatch("activity-status")]
        public async Task<IActionResult> UpdateUserActivityStatus([FromBody] ActivityUpdateDTO dto)
        {
            _logger.LogInformation("Updating user activity status");
            var result = await _userService.UpdateUserActivityStatusAsync(dto.Id, dto.isActive);

            if (!result)
            {
                _logger.LogInformation("User could not be found");
                return NotFound(new { Message = "User not Found!" });
            }

            _logger.LogInformation("User activity status updated successfully");
            return NoContent();
        }
    }
}
