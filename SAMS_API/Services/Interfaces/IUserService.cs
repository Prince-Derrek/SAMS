using SamsApi.DTOs;

namespace SamsApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> RegisterUserAsync(RegisterUserDTO dto);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<PaginatedResponseDTO<UserDTO>> GetPaginatedUsersAsync(int pageIndex, int pageSize);
        Task<UserDTO> GetUserByIdAsync(Guid id);
        Task<bool> UpdateUserActivityStatusAsync(Guid userId, bool isActive);
    }
}
