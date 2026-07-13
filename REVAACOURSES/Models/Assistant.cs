namespace REVAACOURSES.Models
{
    public class Assistant
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public ICollection<Quiez> Quizzes { get; set; } = new List<Quiez>();
    }
}
