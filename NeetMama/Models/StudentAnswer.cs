namespace NeetMama.Models
{
    public class StudentAnswer
    {
        public int Id { get; set; }

        public int AttemptId { get; set; }

        public int QuestionId { get; set; }

        public string SelectedAnswer { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }
}
