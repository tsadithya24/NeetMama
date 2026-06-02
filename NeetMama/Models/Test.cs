namespace NeetMama.Models
{
    public class Test
    {
        public int Id { get; set; }

        public string TestName { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public int DurationMinutes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsPublished { get; set; } = false;
    }
}