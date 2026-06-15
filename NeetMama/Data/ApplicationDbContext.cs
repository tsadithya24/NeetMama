using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NeetMama.Models;

namespace NeetMama.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<UploadedBook> UploadedBooks { get; set; }

        public DbSet<Test> Tests { get; set; }

        public DbSet<TestQuestion> TestQuestions { get; set; }

        public DbSet<StudentTestAttempt> StudentTestAttempts { get; set; }

        public DbSet<StudentAnswer> StudentAnswers { get; set; }

        public DbSet<DocumentChunk> DocumentChunks { get; set; }

        public DbSet<FlashCard> FlashCards { get; set; }

        public DbSet<StudentWeakTopic> StudentWeakTopics { get; set; }
    }
}