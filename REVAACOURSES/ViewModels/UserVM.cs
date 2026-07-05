namespace REVAACOURSES.ViewModels
{
    public class UserVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public List<string> Roles { get; set; }
    }
}
