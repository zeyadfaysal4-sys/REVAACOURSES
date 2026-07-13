namespace REVAACOURSES.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public ICollection<StudentProgress> StudentProgresses { get; set; } = new List<StudentProgress>();

        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    }
}
