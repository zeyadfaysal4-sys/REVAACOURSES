namespace REVAACOURSES.ViewModels
{
    public class LessonWithQuizVM
    {
        public int LessonId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string VideoUrl { get; set; }

        public int CourseId { get; set; }

        // رقم الكويز الخاص بالدرس
        public int? QuizId { get; set; }
    }
}
