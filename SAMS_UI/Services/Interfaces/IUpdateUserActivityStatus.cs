namespace SAMS_UI.Services.Interfaces
{
    public interface IUpdateUserActivityStatus
    {
        Task<bool> UpdateUserActivityStatusAsync(Guid userId, bool isActive);
    }
}
