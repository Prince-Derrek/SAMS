using SAMS_UI.ViewModels;

namespace SAMS_UI.Services.Interfaces
{
    public interface IRegisterUserService
    {
        Task<bool> CreateUserAsync(RegisterViewModel user);
    }
}
