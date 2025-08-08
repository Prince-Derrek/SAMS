using SAMS_UI.Data;
using SAMS_UI.Services.Interfaces;
using SAMS_UI.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace SAMS_UI.Services.Implementations
{
    public class UserQueryService : IUserQueryService
    {
        private readonly AppDbContext _context;

        public UserQueryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedListViewModel<UserViewModel>> GetPaginatedUsersAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.Users.AsNoTracking();

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    UserName = u.Username,
                    UserSecret = u.UserSecret,
                    Role = u.Role.Name,
                    isActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return new PaginatedListViewModel<UserViewModel>
            {
                Items = users,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
