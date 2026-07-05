namespace REVAACOURSES.Models
{
    public class Instructor
    {

        public int Id { get; set; }
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string Bio { get; set; }
        public string PhotoUrl { get; set; }

        public ICollection<Course> Courses { get; set; }
    }
}
