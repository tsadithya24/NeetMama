using System.ComponentModel.DataAnnotations;
namespace NeetMama.Models
{
    public class UploadedBook
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string ClassLevel { get; set; } = string.Empty;

        [Required]
        public string DocumentType { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedDate { get; set; } = DateTime.Now;
    }
}