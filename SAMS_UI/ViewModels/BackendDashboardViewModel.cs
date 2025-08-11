namespace SAMS_UI.ViewModels
{
    public class BackendDashboardViewModel
    {
        public Guid Id
        { get; set; }

        public string UserName
        { get; set; }
        public string UserSecret
        { get; set; }
        public DateTime CreatedAt
        { get; set; }
        public string? Role
        { get; set; }
        public string? Description
        { get; set; }
        public bool isActive
        { get; set; }
    }
}
