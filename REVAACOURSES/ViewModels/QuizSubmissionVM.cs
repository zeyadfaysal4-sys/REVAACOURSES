namespace REVAACOURSES.ViewModels
{
    public class QuizSubmissionVM
    {
        public int QuizId { get; set; }
        public Dictionary<int, string> Answers { get; set; } = new();
    }
}
