namespace REVAACOURSES.Models
{
    public class StudentProgress
    {
        public int Id { get; set; }

        // الطالب
        public int StudentId { get; set; }

        public Student Student { get; set; }

        // الدرس
        public int LessonId { get; set; }

        public Lesson Lesson { get; set; }

        // خلص الدرس؟
        public bool IsCompleted { get; set; } = false;

        // تاريخ الإكمال
        public DateTime? CompletedAt { get; set; }
    }
}
