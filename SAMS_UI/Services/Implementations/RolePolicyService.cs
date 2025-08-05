using Microsoft.EntityFrameworkCore;
using SAMS_UI.Data;
using SAMS_UI.DTOs;
using SAMS_UI.Models;
using SAMS_UI.Services.Interfaces;
using SAMS_UI.ViewModels;

namespace SAMS_UI.Services.Implementations
{
    public class RolePolicyService : IRolePolicyService
    {
        private readonly AppDbContext _context;
        private readonly IRolePolicyQueryService _queryService;
        public RolePolicyService(AppDbContext context, IRolePolicyQueryService queryService)
        {
            _context = context;
            _queryService = queryService;
        }


        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Include(r => r.RolePolicies)
                .ToListAsync();
        }
        public async Task<List<Policy>> GetAllPolicyAsync()
        {
            return await _context.Policies
                .Include(p => p.RolePolicies)
                .ToListAsync();
        }
        public Task<PaginatedListViewModel<RoleViewModel>> GetPaginatedRolesAsync(int pageIndex, int pageSize)
        {
            return _queryService.GetPaginatedRolesAsync(pageIndex, pageSize);
        }
        public Task<PaginatedListViewModel<PolicyViewModel>> GetPaginatedPoliciesAsync(int pageIndex, int pageSize)
        {
            return _queryService.GetPaginatedPoliciesAsync(pageIndex, pageSize);
        }
        public async Task CreateRoleAsync(CreateRoleDTO dto)
        {
            var exists = await _context.Roles
                .AnyAsync(r => r.Name == dto.Name);
            if (!exists)
            {
                _context.Roles
                    .Add(new Role
                    { Name = dto.Name });
                await _context.SaveChangesAsync();
            }
        }
        public async Task CreatePolicyAsync(CreatePolicyDTO dto)
        {
            var exists = await _context.Policies
                .AnyAsync(p => p.Name == dto.Name);
            if (!exists)
            {
                _context.Policies
                    .Add(new Policy { Name = dto.Name, Description = dto.Description });
                await _context.SaveChangesAsync();
            }
        }
        public async Task AssignPolicyToRoleAsync(AssignPolicyToRoleDTO dto)
        {
            var role = await _context.Roles
                .Include(r => r.RolePolicies)
                .FirstOrDefaultAsync(r => r.Id == dto.RoleId);
            if (role == null) return;
            foreach (var policyId in dto.PolicyIds)
            {
                if (!role.RolePolicies.Any(rp => rp.PolicyId == policyId))
                {
                    role.RolePolicies.Add(new RolePolicy { RoleId = role.Id, PolicyId = policyId });
                }
            }
            await _context.SaveChangesAsync();
        }
        public async Task RemovePolicyFromRoleAsync(int roleId, int policyId)
        {
            var existing = await _context.RolePolicies
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PolicyId == policyId);
            if (existing != null)
            {
                _context.RolePolicies.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
        public async Task RemovePolicyAsync(int policyId)
        {
            var existing = await _context.Policies
                .FirstOrDefaultAsync(p => p.Id == policyId);
            if (existing != null)
            {
                _context.Policies.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
        public async Task RemoveRoleAsync(int roleId)
        {
            var existing = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == roleId);
            if (existing != null)
            {
                _context.Roles.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
    }
}
