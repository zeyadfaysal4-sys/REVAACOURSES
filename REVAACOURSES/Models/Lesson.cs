namespace REVAACOURSES.Models
{
    public class Lesson
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string VideoUrl { get; set; }

        public string? PdfFile { get; set; }

        // ترتيب الدرس داخل الكورس
        public int Order { get; set; }

        // هل الطالب يقدر يشوفه قبل شراء الكورس
        public bool IsFreePreview { get; set; } = false;

        // FK
        public int CourseId { get; set; }

        public Course Course { get; set; }

        // One Lesson => One Quiz
        public Quiez? Quiz { get; set; }

        // الطلاب اللى خلصوا الدرس
        public ICollection<StudentProgress> StudentProgresses { get; set; } = new List<StudentProgress>();
    }
}
