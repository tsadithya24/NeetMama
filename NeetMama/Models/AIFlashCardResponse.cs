namespace NeetMama.Models
{
    public class AIFlashCardResponse
    {
        public bool Success { get; set; }

        public List<GeneratedFlashCard> FlashCards { get; set; } = new();
    }

    public class GeneratedFlashCard
    {
        public string FrontText { get; set; } = string.Empty;

        public string BackText { get; set; } = string.Empty;

        public string CardType { get; set; } = "Concept";
    }
}