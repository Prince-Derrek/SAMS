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

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Policy = "CanRegisterUsers")]
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterUserDTO dto)
        {
            var result = await _userService.RegisterUserAsync(dto);
            return Ok(result);
        }

        [Authorize(Policy = "CanViewUsers")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResponseDTO<UserDTO>>> GetPaginatedUsersAsync([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var users = await _userService.GetPaginatedUsersAsync(pageIndex, pageSize);
            return Ok(users);
        }

        [Authorize(Policy = "CanViewUsers")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserByID(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [Authorize(Policy = "CanDisableUsers")]
        [HttpPatch("activity-status")]
        public async Task<IActionResult> UpdateUserActivityStatus([FromBody] ActivityUpdateDTO dto)
        {
            var result = await _userService.UpdateUserActivityStatusAsync(dto.Id, dto.isActive);
            if (!result)
                return NotFound(new { Message = "User not Found!" });
            return NoContent();
        }
    }
}
