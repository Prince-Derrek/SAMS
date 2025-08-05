using Microsoft.EntityFrameworkCore;
using SAMS_UI.Data;
using SAMS_UI.Services.Interfaces;
using SAMS_UI.ViewModels;

namespace SAMS_UI.Services.Implementations
{
    public class RolePolicyQueryService : IRolePolicyQueryService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RolePolicyQueryService> _logger;

        public RolePolicyQueryService(AppDbContext context, ILogger<RolePolicyQueryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedListViewModel<RoleViewModel>> GetPaginatedRolesAsync(int pageIndex, int pageSize)
        {
            var query = _context.Roles.AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(r => r.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RoleViewModel
                {
                    Id = r.Id,
                    Name = r.Name
                }).ToListAsync();

            return new PaginatedListViewModel<RoleViewModel>
            {
                Items = items,
                PageIndex = pageIndex,
                TotalCount = totalCount,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<PaginatedListViewModel<PolicyViewModel>> GetPaginatedPoliciesAsync(int pageIndex, int pageSize)
        {
            var query = _context.Policies.AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PolicyViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                }).ToListAsync();

            return new PaginatedListViewModel<PolicyViewModel>
            {
                Items = items,
                PageIndex = pageIndex,
                TotalCount = totalCount,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
    }
}
