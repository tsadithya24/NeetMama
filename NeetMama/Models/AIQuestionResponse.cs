namespace NeetMama.Models
{
    public class AIQuestionResponse
    {
        public bool Success { get; set; }

        public List<GeneratedQuestion> Questions { get; set; }
            = new();
    }

    public class GeneratedQuestion
    {
        public string Question { get; set; } = "";

        public string OptionA { get; set; } = "";

        public string OptionB { get; set; } = "";

        public string OptionC { get; set; } = "";

        public string OptionD { get; set; } = "";

        public string CorrectAnswer { get; set; } = "";

        public string Explanation { get; set; } = "";
    }
}