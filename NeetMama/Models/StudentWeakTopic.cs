namespace NeetMama.Models
{
    public class StudentWeakTopic
    {
        public int Id { get; set; }

        public string StudentEmail { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Topic { get; set; } = string.Empty;

        public int IncorrectCount { get; set; }

        public int TotalAttempts { get; set; }

        public double Accuracy { get; set; }

        public DateTime LastUpdated { get; set; }
            = DateTime.Now;
    }
}
