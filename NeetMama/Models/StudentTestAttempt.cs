namespace NeetMama.Models
{
    public class StudentTestAttempt
    {
        public int Id { get; set; }

        public string StudentEmail { get; set; } = string.Empty;

        public int TestId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int Score { get; set; }

        public int TotalQuestions { get; set; }

        public int DurationTakenSeconds { get; set; }

        public bool SubmittedByTimer { get; set; } = false;
    }
}
