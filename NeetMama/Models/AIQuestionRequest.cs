namespace NeetMama.Models
{
    public class AIQuestionRequest
    {
        public string Subject { get; set; } = "";

        public string Topic { get; set; } = "";

        public string Difficulty { get; set; } = "Medium";

        public int Count { get; set; } = 5;

        public string QuestionType { get; set; } = "MCQ";
    }
}