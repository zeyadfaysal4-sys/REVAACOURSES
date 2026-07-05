namespace REVAACOURSES.Models
{
    public class Quiez
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int AssistantId { get; set; }
        public Assistant Assistant { get; set; }

    }
}
