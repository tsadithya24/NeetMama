using Microsoft.EntityFrameworkCore;
using NeetMama.Models;

namespace NeetMama.Data
{
    public class ApplicationDbContext : DbContext
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
    }
}