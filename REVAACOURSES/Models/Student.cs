namespace REVAACOURSES.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
