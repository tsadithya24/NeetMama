namespace NeetMama.Models
{
    public class Question
    {
        public int Id { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Chapter { get; set; } = string.Empty;

        public string Topic { get; set; } = string.Empty;

        public string QuestionText { get; set; } = string.Empty;

        public string OptionA { get; set; } = string.Empty;

        public string OptionB { get; set; } = string.Empty;

        public string OptionC { get; set; } = string.Empty;

        public string OptionD { get; set; } = string.Empty;

        public string CorrectAnswer { get; set; } = string.Empty;

        public string Explanation { get; set; } = string.Empty;

        public string Difficulty { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string QuestionType { get; set; } = "MCQ";
    }
}