namespace NeetMama.Models
{
    public class AIFlashCardRequest
    {
        public string Subject { get; set; } = string.Empty;

        public string Topic { get; set; } = string.Empty;

        public string CardType { get; set; } = "Concept";

        public int Count { get; set; } = 10;
    }
}