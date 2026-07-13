namespace REVAACOURSES.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Img { get; set; }
        public int Quantity { get; set; }

        public string Description { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? InstructorId { get; set; }

        public Instructor? Instructor { get; set; }

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    }
}
