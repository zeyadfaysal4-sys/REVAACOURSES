namespace REVAACOURSES.Models
{
    public class QuizResult
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int QuizId { get; set; }
        public Quiez Quiz { get; set; }

        public int Score { get; set; }

        public bool Passed { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}
