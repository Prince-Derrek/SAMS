using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SamsApi.Data;
using SamsApi.DTOs;
using SamsApi.Models;
using SamsApi.Services.Interfaces;

namespace SamsApi.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext context, IMapper mapper, ILogger<UserService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        private string GenerateSecret()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "")
                .Replace("+", "")
                .Substring(0, 16);
        }

        public async Task<UserDTO> RegisterUserAsync(RegisterUserDTO dto)
        {
            // var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == dto.role);

            _logger.LogInformation("Registering a new user");
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                UserSecret = GenerateSecret(),
                RoleId = 1,
                isActive = true
            };
            _logger.LogInformation("Input of user details was succesful");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User saved successfully in the Db");

            await _context.Entry(user).Reference(u => u.Role).LoadAsync();

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            _logger.LogInformation("Retrieving List of Users");
            var users = await _context.Users
                .Include(u => u.Role)
                .ToListAsync();
            _logger.LogInformation("List retrieved successfully");

            return _mapper.Map<List<UserDTO>>(users);
        }
        public async Task<PaginatedResponseDTO<UserDTO>> GetPaginatedUsersAsync(int pageIndex, int pageSize)
        {
            // Validate incoming values to avoid negatives or zero
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            _logger.LogInformation($"Querying users based on Page Size: {pageSize} at {DateTime.UtcNow.AddHours(3)}");
            var baseQuery = _context.Users
                .AsNoTracking() // Optimization for read-only
                .Include(u => u.Role)
                .OrderByDescending(u => u.CreatedAt)
                .AsQueryable();
            _logger.LogInformation("Users Queried successfully");

            var totalCount = await baseQuery.CountAsync();
            _logger.LogInformation($"Total users Queried = {totalCount}");

            var pagedUsers = await baseQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userDTOs = _mapper.Map<List<UserDTO>>(pagedUsers);

            _logger.LogInformation("Creating a Paginated Response");
            return new PaginatedResponseDTO<UserDTO>
            {
                Items = userDTOs,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        public async Task<UserDTO> GetUserByIdAsync(Guid id)
        {
            _logger.LogInformation($"Retrieving user: {id}");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            _logger.LogInformation("User retrieved successfully");

            return _mapper.Map<UserDTO>(user);
        }
        public async Task<bool> UpdateUserActivityStatusAsync(Guid Id, bool isActive)
        {
            _logger.LogInformation($"Finding user : {Id} at {DateTime.UtcNow.AddHours(3)} ");
            var user = await _context.Users.FindAsync(Id);
            if (user == null)
            {
                _logger.LogInformation("User not found");
                return false;
            }
            _logger.LogInformation("User found successfully");
            user.isActive = isActive;

            _logger.LogInformation("Updating user Activity Status");
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Activity Status updated and saved successfully");

            return true;
        }
    }
}
