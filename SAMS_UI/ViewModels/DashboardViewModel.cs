using SAMS_UI.DTOs;

namespace SAMS_UI.ViewModels
{
    public class DashboardViewModel
    {
        public string UserName { get; set; }
        public string WelcomeMessage { get; set; }
        public DashboardMetricsDTO Metrics { get; set; }
    }
}
