namespace NeetMama.Models
{
    public class DocumentChunk
    {
        public int Id { get; set; }

        public int UploadedBookId { get; set; }

        public int ChunkNumber { get; set; }

        public string ChunkText { get; set; } = string.Empty;

        public int ChunkLength { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}