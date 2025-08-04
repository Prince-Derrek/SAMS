using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SamsApi.Data;
using SamsApi.DTOs;
using SamsApi.Models;

namespace SamsApi.Services.Implementations
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                UserSecret = GenerateSecret(),
                RoleId = 1,
                isActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _context.Entry(user).Reference(u => u.Role).LoadAsync();

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .ToListAsync();

            return _mapper.Map<List<UserDTO>>(users);
        }
        public async Task<PaginatedResponseDTO<UserDTO>> GetPaginatedUsersAsync(int pageIndex, int pageSize)
        {
            // Validate incoming values to avoid negatives or zero
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var baseQuery = _context.Users
                .AsNoTracking() // Optimization for read-only
                .Include(u => u.Role)
                .OrderByDescending(u => u.CreatedAt)
                .AsQueryable();

            var totalCount = await baseQuery.CountAsync();

            var pagedUsers = await baseQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userDTOs = _mapper.Map<List<UserDTO>>(pagedUsers);

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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            return _mapper.Map<UserDTO>(user);
        }
        public async Task<bool> UpdateUserActivityStatusAsync(Guid userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;
            user.isActive = isActive;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
