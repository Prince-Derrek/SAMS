using SAMS_UI.ViewModels;

namespace SAMS_UI.Services.Interfaces
{
    public interface IGetUserById
    {
        Task<UserViewModel> GetUserByIdAsync(Guid id);
    }
}
