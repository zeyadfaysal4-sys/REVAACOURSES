namespace REVAACOURSES.Models
{
    public class Quiez
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();

    }
}
