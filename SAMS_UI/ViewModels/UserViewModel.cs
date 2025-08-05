namespace SAMS_UI.ViewModels
{
    public class UserViewModel
    {
        public Guid userId
        { get; set; }

        public string userName
        { get; set; }
        public string userSecret
        { get; set; }
        public DateTime userCreatedAt
        { get; set; }
        public string Role
        { get; set; }
        public string Description
        { get; set; }
        public bool isActive
        { get; set; }
    }
}
