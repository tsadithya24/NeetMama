namespace NeetMama.Models
{
    public class FlashCard
    {
        public int Id { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Chapter { get; set; } = string.Empty;

        public string Topic { get; set; } = string.Empty;

        public string FrontText { get; set; } = string.Empty;

        public string BackText { get; set; } = string.Empty;

        public string CardType { get; set; } = "Concept";

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}