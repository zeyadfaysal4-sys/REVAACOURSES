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

    }
}
