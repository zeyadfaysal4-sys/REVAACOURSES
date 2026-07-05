namespace REVAACOURSES.Models
{
    public class Assistant
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
