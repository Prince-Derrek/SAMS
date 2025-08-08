namespace SAMS_UI.Services.Interfaces
{
    public interface IUpdateUserActivityStatus
    {
        Task<bool> UpdateFrontendUserActivityStatusAsync(Guid userId, bool isActive);
        Task<bool> UpdateBackendUserActivityStatusAsync(Guid userId, bool isActive);
    }
}
