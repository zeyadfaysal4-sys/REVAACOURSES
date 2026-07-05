namespace REVAACOURSES.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool status { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
