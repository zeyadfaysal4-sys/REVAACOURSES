namespace REVAACOURSES.Models
{
    public class Certificate
    {
        public int Id { get; set; }

        // رقم الشهادة
        public string CertificateNumber { get; set; }

        public DateTime IssueDate { get; set; } = DateTime.Now;

        // الطالب
        public int StudentId { get; set; }

        public Student Student { get; set; }

        // الكورس
        public int CourseId { get; set; }

        public Course Course { get; set; }
    }
}
