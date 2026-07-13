namespace REVAACOURSES.Models
{
    public enum AnswerOption
    {
        A,
        B,
        C,
        D
    }

    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public AnswerOption CorrectAnswer { get; set; }
        public int QuizId { get; set; }
        public Quiez Quiz { get; set; }
    }
}